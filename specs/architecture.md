# Arquitetura — Agente de Classificação Fiscal de Produtos

> Deriva da [Constituição](./constitution.md) e dos [Requisitos](./requirements.md). Descreve a arquitetura-alvo e o stack tecnológico. **Nenhum código é implementado nesta fase.**

| Campo | Valor |
|-------|-------|
| **Agente** | Classificação Fiscal de Produtos |
| **Responsável** | Marcus Oliveira |
| **Orquestrador** | SOFIA (integração futura) |
| **Versão** | 0.2.0 |
| **Data** | 2026-10-22 |

---

## 1. Visão Geral

O agente é um **serviço especialista isolado** que recebe descrições de produtos e devolve classificação fiscal (NCM/CEST) fundamentada e auditada. É exposto como uma **API ASP.NET Core (.NET 10)**, orquestra o raciocínio via **Microsoft Semantic Kernel** sobre modelos **OpenAI**, recupera fundamentação por **RAG com PgVector** e mantém **memória conversacional em MongoDB**. Opera de forma autônoma e, futuramente, como um agente registrado no orquestrador **SOFIA**.

```
                         ┌──────────────────────────┐
   Usuário / Sistema ───▶│                          │
                         │     SOFIA (futuro)       │
   (trace id, auth)      │   Orquestrador central   │
                         └────────────┬─────────────┘
                                      │ contrato versionado (P-INT-1)
                                      ▼
┌───────────────────────────────────────────────────────────────────┐
│   Agente: Classificação Fiscal de Produtos                          │
│   (ASP.NET Core API · .NET 10 · Swagger/OpenAPI)                    │
│                                                                     │
│  ┌──────────────┐  ┌───────────────┐  ┌────────────────────────┐   │
│  │ API / Adapter│─▶│ Validação &   │─▶│ Núcleo de Classificação│   │
│  │ (controllers)│  │ Sanitização   │  │ (Semantic Kernel)      │   │
│  └──────────────┘  └───────────────┘  └───────┬──────┬─────────┘   │
│         │                                      │      │             │
│         │                         LLM (OpenAI) │      │ RAG         │
│         │                                      ▼      ▼             │
│         │                         ┌──────────────┐ ┌────────────┐   │
│         │                         │ OpenAI       │ │ PgVector    │  │
│         │                         │ (chat/embed) │ │ (base NCM/  │  │
│         │                         └──────────────┘ │  CEST + leis)│ │
│         │                                          └────────────┘   │
│         │            Memória conversacional                         │
│         │                     ▼                                     │
│         │            ┌──────────────┐                               │
│         │            │ MongoDB      │                               │
│         │            └──────────────┘                               │
│         ▼                                                            │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │ Trilha de Auditoria (append-only) + Observabilidade           │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  Transversais: Segredos (cofre) · Isolamento multi-tenant ·         │
│                Menor privilégio · Criptografia                      │
│  Qualidade: Testes com xUnit                                        │
└───────────────────────────────────────────────────────────────────┘
```

---

## 2. Stack Tecnológico

| Camada | Tecnologia | Papel | Princípios/Requisitos |
|--------|-----------|-------|-----------------------|
| **API / Runtime** | **.NET 10 — ASP.NET Core Web API** | Hospeda o agente, expõe endpoints do contrato e o manifesto de capacidades. | P-INT-1, P-INT-2, RF-08 |
| **Documentação de API** | **Swagger / OpenAPI** | Contrato versionado navegável e descoberta por SOFIA. | P-INT-1, P-INT-5, RF-08 |
| **Orquestração de IA** | **Microsoft Semantic Kernel** | Coordena prompts, *plugins/functions*, memória e pipeline RAG; isola o domínio da infraestrutura de LLM. | P-ISO-1, RF-01, RF-03 |
| **LLM** | **OpenAI** (chat + *embeddings*) | Raciocínio de classificação e geração de *embeddings* para RAG. | RF-01, RF-03, RF-05 |
| **RAG / Vetores** | **PostgreSQL + PgVector** | Armazena *embeddings* da base legal (NCM/CEST, notas explicativas) e faz busca por similaridade. | ADR-03, C-02, RF-10, RF-11 |
| **Memória Conversacional** | **MongoDB** | Persiste histórico/contexto de conversas, isolado por tenant. | P-ISO-2, P-ISO-3, P-AUD-5 |
| **Testes** | **xUnit** | Testes unitários e de integração; base da Definição de Pronto. | DoD (plan.md) |
| **Segredos** | Cofre de segredos (infra) | Chaves OpenAI, conexões de banco. | P-SEC-3 |

> Observação: a **base de conhecimento** é materializada no **PgVector** (busca semântica) e pode combinar metadados relacionais no mesmo PostgreSQL. A **memória conversacional** fica no **MongoDB**, separada da base legal e da trilha de auditoria.

---

## 3. Componentes

### 3.1 API / Adapter de Interface — *ASP.NET Core (.NET 10)*
- Controllers/*minimal APIs* expõem o **contrato de entrada/saída versionado** (`P-INT-1`, `RNF-INT-03`).
- **Swagger/OpenAPI** publica o contrato e apoia o **manifesto de capacidades** para descoberta por SOFIA (`RF-08`, `P-INT-2`).
- Middleware preserva e propaga o **trace id** (`RF-09`, `P-INT-4`).
- Garante **idempotência** por chave de correlação (`RNF-INT-02`).

### 3.2 Validação & Sanitização
- Trata toda entrada como não confiável (`P-SEC-2`, `P-SEC-6`), inclusive descrições de produto (defesa contra *prompt injection*).
- Validação de modelo (*model binding*/validators) antes de alcançar o núcleo.

### 3.3 Núcleo de Classificação — *Semantic Kernel + OpenAI*
- Orquestra o fluxo de classificação com **Semantic Kernel** (prompts, *functions*, memória, RAG).
- Usa **OpenAI** para chat e *embeddings* (`RF-01`, `RF-02`).
- Produz **grau de confiança** e **candidatos alternativos** (`RF-03`, `RF-05`).
- Ancora respostas na recuperação do PgVector — **não inventa códigos** (`C-02`, `ADR-03`).

### 3.4 RAG / Base de Conhecimento — *PgVector*
- Fonte de verdade **versionada** das tabelas NCM/CEST e notas explicativas, indexada por *embeddings* (`A-02`, `RF-10`).
- Busca por similaridade fornece trechos de fundamentação ao núcleo.
- Sinaliza desatualização/indisponibilidade (`RF-11`). Somente leitura para o fluxo de classificação.

### 3.5 Memória Conversacional — *MongoDB*
- Persiste histórico e contexto de conversas, **isolado por tenant** (`P-ISO-3`).
- Estado persistente é explicitamente declarado e versionado (`P-ISO-2`).
- Dados sensíveis minimizados (LGPD) e sujeitos à política de retenção.

### 3.6 Trilha de Auditoria
- Registro **append-only** e imutável por classificação (`P-AUD-1`, `P-AUD-3`).
- Guarda entrada, saída, versões (agente, base, **modelo OpenAI**), timestamp UTC, origem e trace id (`P-AUD-5`).
- Separada da memória conversacional e dos logs técnicos (`P-AUD-4`).

### 3.7 Observabilidade
- Métricas, logs técnicos (com mascaramento) e *tracing* correlacionados por trace id (`RNF-OBS-01/02`, `P-AUD-4`).

### 3.8 Qualidade — *xUnit*
- Testes unitários (validação, mapeamento de contrato, regras) e de integração (PgVector, MongoDB, camada OpenAI com *mocks/fakes*).
- Cobertura compõe a Definição de Pronto de cada fase (ver `plan.md`).

### 3.9 Transversais
- **Cofre de segredos** (`P-SEC-3`), **criptografia** em trânsito/repouso (`P-SEC-4`), **isolamento multi-tenant** (`P-ISO-3`), **menor privilégio** (`P-SEC-1`).

---

## 4. Contrato de Interface (esboço)

> Estrutura ilustrativa — a especificação formal é publicada via **Swagger/OpenAPI** na Fase 1.

**Requisição**
```json
{
  "trace_id": "string (propagado por SOFIA ou gerado)",
  "tenant_id": "string",
  "idempotency_key": "string",
  "conversation_id": "string | null",
  "itens": [
    {
      "id": "string",
      "descricao": "string",
      "atributos": { "marca": "string", "material": "string", "aplicacao": "string" },
      "ncm_candidato": "string | null"
    }
  ]
}
```

**Resposta**
```json
{
  "trace_id": "string",
  "versao_agente": "semver",
  "versao_base_legal": "string",
  "modelo_llm": "string (ex.: openai:<modelo>)",
  "resultados": [
    {
      "id": "string",
      "ncm": "string",
      "cest": "string | null",
      "confianca": "alto | medio | baixo",
      "alternativas": [ { "ncm": "string", "confianca": "..." } ],
      "fundamentacao": "string (posição/capítulo/nota/regra + trechos recuperados via RAG)",
      "status": "classificado | recusado | ambiguo"
    }
  ]
}
```

---

## 5. Decisões Arquiteturais (ADRs — resumo)

| ID | Decisão | Motivação | Princípios |
|----|---------|-----------|------------|
| **ADR-01** | Serviço especialista isolado, stateless por padrão (memória externa no MongoDB). | Isolamento de falhas e de dados. | P-ISO-2, P-ISO-5 |
| **ADR-02** | Contrato versionado + manifesto de capacidades via Swagger/OpenAPI. | Integração limpa com SOFIA. | P-INT-1, P-INT-2 |
| **ADR-03** | Base de conhecimento versionada em PgVector como fonte de verdade para RAG. | Rastreabilidade e não alucinação de códigos. | P-AUD-2, C-02 |
| **ADR-04** | Auditoria append-only separada dos logs técnicos e da memória conversacional. | Conformidade e imutabilidade. | P-AUD-1, P-AUD-4 |
| **ADR-05** | Validação de toda entrada, inclusive a vinda de SOFIA. | Zero-trust entre agentes. | P-SEC-2, P-INT-6 |
| **ADR-06** | Idempotência por chave de correlação. | Reprocessamento seguro em orquestração. | P-INT-3 |
| **ADR-07** | API em **.NET 10 / ASP.NET Core**. | Plataforma corporativa, desempenho e suporte. | RNF-PERF-* |
| **ADR-08** | Orquestração de IA via **Semantic Kernel**. | Abstrai LLM, memória e RAG; mantém domínio isolado da infraestrutura. | P-ISO-1 |
| **ADR-09** | **OpenAI** como provedor de LLM e *embeddings*. | Qualidade de raciocínio e ecossistema com Semantic Kernel. | RF-01, RF-03 |
| **ADR-10** | **MongoDB** para memória conversacional, isolada por tenant. | Flexibilidade de esquema e isolamento de estado. | P-ISO-2, P-ISO-3 |
| **ADR-11** | **xUnit** como framework de testes. | Padrão do ecossistema .NET; apoia a DoD. | DoD |

---

## 6. Fluxo de uma Classificação

1. Chamador (usuário/sistema/SOFIA) envia requisição à **API ASP.NET Core** com `trace_id`, `tenant_id` e itens.
2. **Validação & Sanitização** verifica e normaliza; rejeita entradas inválidas.
3. **Núcleo (Semantic Kernel)** recupera fundamentação no **PgVector** (RAG) e consulta a **OpenAI** para determinar NCM/CEST, confiança e alternativas.
4. Contexto da conversa é lido/gravado no **MongoDB** quando houver `conversation_id`.
5. Resposta estruturada é montada com fundamentação, versões e modelo utilizado.
6. **Trilha de Auditoria** registra o evento (imutável).
7. **Observabilidade** emite métricas/tracing correlacionados pelo `trace_id`.

---

## 7. Integração com SOFIA (futura)

- Registro do agente no orquestrador via **manifesto de capacidades** (exposto por Swagger/OpenAPI).
- Comunicação por contrato versionado; SOFIA trata o agente como caixa-preta especialista.
- *Trace id* propagado ponta a ponta.
- Agente mantém autonomia: deve operar mesmo sem SOFIA (`C-04`).
- Zero-trust: entradas de SOFIA validadas como qualquer outra (`P-INT-6`).

---

## 8. Histórico de Revisões

| Versão | Data | Autor | Descrição |
|--------|------|-------|-----------|
| 0.1.0 | 2026-10-22 | Marcus Oliveira | Estrutura inicial da arquitetura (SDD). |
| 0.2.0 | 2026-10-22 | Marcus Oliveira | Definição do stack: .NET 10/ASP.NET Core, OpenAI, RAG com PgVector, memória em MongoDB, Semantic Kernel, Swagger e xUnit. |
