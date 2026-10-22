# Plano de Implementação — Agente de Classificação Fiscal de Produtos

> Deriva da [Constituição](./constitution.md), dos [Requisitos](./requirements.md) e da [Arquitetura](./architecture.md). Define fases, marcos e critérios de saída. **Esta etapa é somente de planejamento — sem implementação de código.**

| Campo | Valor |
|-------|-------|
| **Agente** | Classificação Fiscal de Produtos |
| **Responsável** | Marcus Oliveira |
| **Orquestrador** | SOFIA (integração futura) |
| **Versão** | 0.1.0 |
| **Data** | 2026-10-22 |

---

## 1. Objetivo do Plano

Levar o agente da especificação (atual) até um serviço operacional, auditável e integrável a SOFIA, respeitando os princípios de isolamento, auditoria, segurança e integração multi-agente.

---

## 2. Fases

### Fase 0 — Especificação SDD *(em andamento)*
- **Escopo:** Constituição, Requisitos, Arquitetura, Plano, Tarefas, AGENTS.md.
- **Entregáveis:** documentos `specs/*` + `AGENTS.md`.
- **Critério de saída:** documentos revisados e aprovados pelo Responsável.
- **Status:** ativo (sem código).

### Fase 1 — Fundações e Contrato
- **Escopo:** definir o contrato formal de interface (JSON Schema/OpenAPI), manifesto de capacidades, estrutura de projeto, política de segredos e tenancy.
- **Princípios-chave:** `P-INT-1`, `P-INT-2`, `P-SEC-3`, `P-ISO-3`.
- **Critério de saída:** contrato versionado publicado; esqueleto de projeto sem lógica de domínio.

### Fase 2 — Base de Conhecimento
- **Escopo:** ingestão e versionamento das tabelas NCM/CEST e notas explicativas; sinalização de atualização/indisponibilidade.
- **Princípios-chave:** `P-AUD-2`, `C-02`, `RF-10`, `RF-11`.
- **Critério de saída:** base versionada consultável (somente leitura) com metadados de versão.

### Fase 3 — Núcleo de Classificação (MVP)
- **Escopo:** classificação individual (NCM), confiança e fundamentação; recusa de fora de escopo.
- **Princípios-chave:** `RF-01`, `RF-03`, `RF-06`, `P-ISO-1`.
- **Critério de saída:** classificação individual funcionando com saída estruturada.

### Fase 4 — Auditoria e Observabilidade
- **Escopo:** trilha append-only, logs técnicos mascarados, métricas e tracing por trace id.
- **Princípios-chave:** `P-AUD-1..5`, `RNF-OBS-01/02`.
- **Critério de saída:** toda classificação gera auditoria imutável correlacionada.

### Fase 5 — CEST, Lote e Alternativas
- **Escopo:** CEST, processamento em lote, candidatos alternativos ranqueados, idempotência.
- **Princípios-chave:** `RF-02`, `RF-04`, `RF-05`, `RNF-INT-02`.
- **Critério de saída:** metas de lote (`RNF-PERF-03`) atendidas.

### Fase 6 — Segurança e Conformidade (Hardening)
- **Escopo:** validação/sanitização reforçada, criptografia, menor privilégio, LGPD, testes de *prompt injection*.
- **Princípios-chave:** `P-SEC-1..6`, `C-03`.
- **Critério de saída:** revisão de segurança aprovada; nenhum segredo em código/logs.

### Fase 7 — Preparação para Integração com SOFIA
- **Escopo:** registro via manifesto, propagação de trace id ponta a ponta, convivência de versões, operação autônoma sem SOFIA.
- **Princípios-chave:** `P-INT-1..6`, `C-04`.
- **Critério de saída:** agente registrável e consumível por SOFIA em ambiente de homologação.

---

## 3. Marcos (Milestones)

| Marco | Descrição | Depende de |
|-------|-----------|------------|
| **M0** | SDD aprovada | Fase 0 |
| **M1** | Contrato + esqueleto | Fase 1 |
| **M2** | Base NCM/CEST versionada | Fase 2 |
| **M3** | MVP de classificação | Fases 3, 4 |
| **M4** | Lote + CEST + alternativas | Fase 5 |
| **M5** | Hardening aprovado | Fase 6 |
| **M6** | Integração SOFIA (homologação) | Fase 7 |

---

## 4. Dependências e Premissas
- Fonte oficial e atualizada de tabelas NCM/CEST (`A-02`).
- Definição de autenticação de serviço fornecida por SOFIA (`A-01`).
- Cofre de segredos disponível na infraestrutura (`P-SEC-3`).

---

## 5. Riscos

| ID | Risco | Mitigação |
|----|-------|-----------|
| **R-01** | Desatualização das tabelas legais. | Versionamento + sinalização (`RF-11`); processo de atualização. |
| **R-02** | Alucinação de códigos pelo modelo. | Ancoragem na base de conhecimento (`C-02`, `ADR-03`). |
| **R-03** | Vazamento entre tenants. | Isolamento lógico e testes (`P-ISO-3`). |
| **R-04** | *Prompt injection* via descrição de produto. | Validação/sanitização e zero-trust (`P-SEC-2`, `P-SEC-6`). |
| **R-05** | Quebra de contrato com SOFIA. | SemVer + convivência de versões (`P-INT-5`). |

---

## 6. Definição de Pronto (DoD) por fase
- Requisitos atendidos referenciados e verificados.
- Auditoria e segurança aplicáveis cobertas.
- Documentação SDD atualizada.
- Aprovação do Responsável.

---

## 7. Histórico de Revisões

| Versão | Data | Autor | Descrição |
|--------|------|-------|-----------|
| 0.1.0 | 2026-10-22 | Marcus Oliveira | Estrutura inicial do plano (SDD). |
