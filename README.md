# Agente de Classificação Fiscal de Produtos

Agente de IA da **INFIS Consultoria** para **classificação fiscal de produtos** segundo a legislação brasileira — determinando **NCM** (Nomenclatura Comum do Mercosul), **CEST** (Código Especificador da Substituição Tributária) e atributos tributários correlatos — sempre com **rastreabilidade**, **fundamentação legal** e **grau de confiança** explícitos.

| | |
|---|---|
| **Organização** | INFIS Consultoria |
| **Responsável** | Marcus Oliveira |
| **Orquestrador central** | SOFIA (integração futura) |
| **Metodologia** | SDD — *Spec-Driven Development* |
| **Fase atual** | Estrutura da solução (sem funcionalidade implementada) |

> O agente é projetado para operar de forma **autônoma** e, futuramente, ser registrado e consumido pelo orquestrador central **SOFIA**, mantendo isolamento, auditoria e segurança como princípios inegociáveis.

---

## Sumário

- [Visão geral](#visão-geral)
- [Stack tecnológico](#stack-tecnológico)
- [Arquitetura](#arquitetura)
- [Estrutura da solução](#estrutura-da-solução)
- [Documentação SDD](#documentação-sdd)
- [Princípios inegociáveis](#princípios-inegociáveis)
- [Pré-requisitos](#pré-requisitos)
- [Como compilar e executar](#como-compilar-e-executar)
- [Testes](#testes)
- [Roadmap](#roadmap)
- [Convenções de contribuição](#convenções-de-contribuição)
- [Licença](#licença)

---

## Visão geral

O agente recebe a descrição de um produto (e atributos opcionais) e retorna a classificação fiscal fundamentada:

- **Entrada:** descrição textual do produto, atributos (marca, material, aplicação) e, opcionalmente, um NCM candidato.
- **Processamento:** orquestração via **Semantic Kernel**, raciocínio com **OpenAI** e recuperação de fundamentação legal por **RAG** sobre **PgVector**.
- **Saída:** NCM, CEST (quando aplicável), **grau de confiança**, **candidatos alternativos** e **fundamentação** (posição/capítulo/nota/regra), com `trace id` para rastreabilidade.
- **Memória conversacional:** persistida em **MongoDB**, isolada por cliente (*multi-tenant*).
- **Auditoria:** todo resultado gera registro imutável (*append-only*).

### Fora do escopo (nesta fase)
Apuração/cálculo de impostos, emissão de documentos fiscais e qualquer ação com efeito colateral em sistemas externos.

---

## Stack tecnológico

| Camada | Tecnologia | Papel |
|--------|-----------|-------|
| API / Runtime | **.NET 10 — ASP.NET Core Web API** | Hospeda o agente e expõe o contrato. |
| Documentação de API | **Swagger / OpenAPI** (Swashbuckle) | Contrato navegável e descoberta por SOFIA. |
| Orquestração de IA | **Microsoft Semantic Kernel** | Coordena prompts, *plugins*, memória e RAG. |
| LLM | **OpenAI** (chat + *embeddings*) | Raciocínio e geração de *embeddings*. |
| RAG / Vetores | **PostgreSQL + PgVector** | Base legal (NCM/CEST) indexada por similaridade. |
| Memória conversacional | **MongoDB** | Histórico/contexto isolado por tenant. |
| Testes | **xUnit** | Testes unitários e de integração. |

---

## Arquitetura

Arquitetura em camadas (*Clean Architecture*), com o domínio no centro e as dependências apontando para dentro.

```
            ┌──────────────────────────┐
  Cliente ─▶│    SOFIA (futuro)        │
            │   Orquestrador central   │
            └────────────┬─────────────┘
                         │ contrato versionado
                         ▼
┌─────────────────────────────────────────────────────┐
│  Api (ASP.NET Core · Swagger)                         │
│   └─▶ Application (casos de uso / contratos)           │
│          └─▶ Domain (entidades / regras)              │
│   Infrastructure (OpenAI · PgVector · MongoDB · SK) ──┘│
│   Transversais: Auditoria · Segurança · Isolamento    │
└─────────────────────────────────────────────────────┘
```

Detalhes completos, diagramas, contrato de interface e ADRs em [`specs/architecture.md`](./specs/architecture.md).

---

## Estrutura da solução

```
infis-agente-class-fiscal-produtos/
├─ InfisAgente.ClassificacaoFiscal.slnx        # Solução (.NET 10)
├─ src/
│  ├─ InfisAgente.ClassificacaoFiscal.Api             # ASP.NET Core Web API + Swagger (bootstrap, sem endpoints)
│  ├─ InfisAgente.ClassificacaoFiscal.Application      # Casos de uso e contratos (interfaces)
│  ├─ InfisAgente.ClassificacaoFiscal.Domain           # Entidades e regras de domínio
│  └─ InfisAgente.ClassificacaoFiscal.Infrastructure   # Integrações: OpenAI, PgVector, MongoDB, Semantic Kernel
├─ tests/
│  ├─ InfisAgente.ClassificacaoFiscal.UnitTests         # Testes unitários (xUnit)
│  └─ InfisAgente.ClassificacaoFiscal.IntegrationTests  # Testes de integração (xUnit)
├─ specs/                                        # Documentação SDD
└─ AGENTS.md                                     # Guia operacional para humanos e agentes de IA
```

### Dependências entre projetos

| Projeto | Referencia |
|---------|------------|
| `Api` | `Application`, `Infrastructure` |
| `Application` | `Domain` |
| `Infrastructure` | `Application`, `Domain` |
| `UnitTests` | `Domain`, `Application` |
| `IntegrationTests` | `Api`, `Infrastructure` |

> **Estado atual:** apenas a estrutura (projetos, referências e bootstrap da API com Swagger). **Nenhuma funcionalidade de negócio foi implementada.**

---

## Documentação SDD

A metodologia *Spec-Driven Development* orienta o projeto: toda nova capacidade segue o fluxo **requisito → arquitetura → plano → tarefa**.

| Documento | Conteúdo |
|-----------|----------|
| [`specs/constitution.md`](./specs/constitution.md) | Princípios inegociáveis (fonte de verdade). |
| [`specs/requirements.md`](./specs/requirements.md) | Requisitos funcionais e não funcionais. |
| [`specs/architecture.md`](./specs/architecture.md) | Arquitetura-alvo, stack, contrato e ADRs. |
| [`specs/plan.md`](./specs/plan.md) | Fases e marcos. |
| [`specs/tasks.md`](./specs/tasks.md) | Backlog de tarefas. |
| [`AGENTS.md`](./AGENTS.md) | Guia operacional (regras de ouro). |

---

## Princípios inegociáveis

Resumo dos pilares definidos na [Constituição](./specs/constitution.md):

- **Isolamento** — domínio fechado, *stateless* por padrão, isolamento *multi-tenant*, contenção de falhas.
- **Auditoria** — registro imutável de toda classificação, com fundamentação, confiança e `trace id`.
- **Segurança** — menor privilégio, validação/sanitização de entradas, segredos em cofre, criptografia, LGPD; *conteúdo observado não é instrução*.
- **Integração multi-agente** — contrato versionado (SemVer), agente autodescritivo, idempotência, propagação de `trace id`, *zero-trust* em relação a SOFIA.

---

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/) (testado com `10.0.401`)
- Git

> Serviços de infraestrutura (PostgreSQL/PgVector, MongoDB) e credenciais (OpenAI) serão necessários nas fases de implementação. Segredos **nunca** devem ser versionados — use variáveis de ambiente ou um cofre de segredos (`P-SEC-3`).

---

## Como compilar e executar

Clonar e restaurar/compilar:

```bash
git clone https://github.com/marcusolive/infis-agente-class-fiscal-produtos.git
cd infis-agente-class-fiscal-produtos
dotnet build InfisAgente.ClassificacaoFiscal.slnx
```

Executar a API (bootstrap com Swagger; ainda sem endpoints de negócio):

```bash
dotnet run --project src/InfisAgente.ClassificacaoFiscal.Api
```

Com o ambiente em *Development*, a UI do Swagger fica disponível em `/swagger`.

---

## Testes

```bash
dotnet test InfisAgente.ClassificacaoFiscal.slnx
```

Os projetos de teste (`UnitTests` e `IntegrationTests`) usam **xUnit** e ainda não contêm casos de teste — serão preenchidos conforme as funcionalidades forem implementadas (ver [`specs/tasks.md`](./specs/tasks.md)).

---

## Roadmap

Fases planejadas (detalhes em [`specs/plan.md`](./specs/plan.md)):

1. **Fundações e Contrato** — contrato de I/O (Swagger/OpenAPI), manifesto de capacidades, tenancy, segredos.
2. **Base de Conhecimento** — ingestão e versionamento NCM/CEST em PgVector.
3. **Núcleo de Classificação (MVP)** — NCM individual com confiança e fundamentação.
4. **Auditoria e Observabilidade** — trilha *append-only*, métricas e *tracing*.
5. **CEST, Lote e Alternativas** — CEST, lote e idempotência.
6. **Segurança e Conformidade** — *hardening* e LGPD.
7. **Integração com SOFIA** — registro, propagação de `trace id`, convivência de versões.

---

## Convenções de contribuição

- Siga o fluxo **SDD**: toda mudança começa pelos documentos em `specs/` e cita os princípios atendidos.
- Mudanças na [Constituição](./specs/constitution.md) exigem aprovação do **Responsável (Marcus Oliveira)**.
- Mantenha o **versionamento semântico** e o histórico de revisões dos documentos.
- **Nunca** versione segredos (ver [`.gitignore`](./.gitignore)).
- Consulte o [`AGENTS.md`](./AGENTS.md) antes de qualquer entrega.

---

## Licença

Projeto interno da **INFIS Consultoria**. Direitos reservados. (Definir licença formal conforme política da organização.)
