# Tarefas — Agente de Classificação Fiscal de Produtos

> Deriva do [Plano](./plan.md). Backlog organizado por fase. Cada tarefa referencia requisitos/princípios. **Nenhuma tarefa de implementação de código é executada nesta etapa** — este é o backlog planejado.

| Campo | Valor |
|-------|-------|
| **Agente** | Classificação Fiscal de Produtos |
| **Responsável** | Marcus Oliveira |
| **Orquestrador** | SOFIA (integração futura) |
| **Versão** | 0.1.0 |
| **Data** | 2026-10-22 |

**Legenda de status:** ⬜ pendente · 🟡 em andamento · ✅ concluído

---

## Fase 0 — Especificação SDD

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T0-01 | Redigir Constituição | constitution.md | ✅ |
| T0-02 | Redigir Requisitos | requirements.md | ✅ |
| T0-03 | Redigir Arquitetura | architecture.md | ✅ |
| T0-04 | Redigir Plano | plan.md | ✅ |
| T0-05 | Redigir Tarefas (este documento) | tasks.md | ✅ |
| T0-06 | Redigir AGENTS.md | AGENTS.md | ✅ |
| T0-07 | Revisão e aprovação do Responsável | M0 | ⬜ |

---

## Fase 1 — Fundações e Contrato

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T1-01 | Especificar contrato formal de I/O (JSON Schema/OpenAPI) | RF-08, P-INT-1 | ⬜ |
| T1-02 | Definir manifesto de capacidades do agente | RF-08, P-INT-2 | ⬜ |
| T1-03 | Definir estrutura de projeto e padrões de código | — | ⬜ |
| T1-04 | Definir política de segredos (cofre, rotação) | P-SEC-3 | ⬜ |
| T1-05 | Definir modelo multi-tenant e isolamento de dados | P-ISO-3 | ⬜ |
| T1-06 | Definir esquema de idempotência e trace id | P-INT-3, P-INT-4 | ⬜ |

---

## Fase 2 — Base de Conhecimento

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T2-01 | Definir fonte oficial e processo de ingestão NCM/CEST | A-02, C-02 | ⬜ |
| T2-02 | Modelar versionamento da base legal | RF-10 | ⬜ |
| T2-03 | Implementar sinalização de desatualização/indisponibilidade | RF-11 | ⬜ |
| T2-04 | Garantir acesso somente leitura ao agente | P-SEC-1 | ⬜ |

---

## Fase 3 — Núcleo de Classificação (MVP)

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T3-01 | Classificação NCM individual | RF-01 | ⬜ |
| T3-02 | Cálculo de grau de confiança | RF-03 | ⬜ |
| T3-03 | Geração de fundamentação legal | RF-03, P-AUD-2 | ⬜ |
| T3-04 | Recusa rastreável de fora de escopo | RF-06, P-ISO-1 | ⬜ |
| T3-05 | Saída estruturada conforme contrato | T1-01 | ⬜ |

---

## Fase 4 — Auditoria e Observabilidade

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T4-01 | Trilha de auditoria append-only | P-AUD-1, P-AUD-3 | ⬜ |
| T4-02 | Registro de versões (agente/base/modelo) por evento | P-AUD-1 | ⬜ |
| T4-03 | Logs técnicos com mascaramento de dados sensíveis | P-AUD-4 | ⬜ |
| T4-04 | Métricas (volume, latência, recusa, confiança) | RNF-OBS-01 | ⬜ |
| T4-05 | Tracing correlacionado por trace id | RNF-OBS-02 | ⬜ |
| T4-06 | Política de retenção de auditoria | RNF-AUD-02 | ⬜ |

---

## Fase 5 — CEST, Lote e Alternativas

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T5-01 | Determinação de CEST | RF-02 | ⬜ |
| T5-02 | Classificação em lote | RF-04 | ⬜ |
| T5-03 | Candidatos alternativos ranqueados | RF-05 | ⬜ |
| T5-04 | Idempotência por chave de correlação | RNF-INT-02 | ⬜ |
| T5-05 | Validar metas de desempenho de lote | RNF-PERF-03 | ⬜ |

---

## Fase 6 — Segurança e Conformidade

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T6-01 | Validação/sanitização reforçada de entradas | P-SEC-2, P-SEC-6 | ⬜ |
| T6-02 | Criptografia em trânsito e repouso | P-SEC-4 | ⬜ |
| T6-03 | Menor privilégio aplicado a serviços/dados | P-SEC-1 | ⬜ |
| T6-04 | Conformidade LGPD (minimização de PII) | C-03 | ⬜ |
| T6-05 | Testes de prompt injection e abuso | P-SEC-6 | ⬜ |
| T6-06 | Revisão de segurança final | M5 | ⬜ |

---

## Fase 7 — Integração com SOFIA

| ID | Tarefa | Ref. | Status |
|----|--------|------|--------|
| T7-01 | Registro do agente via manifesto em SOFIA | P-INT-2 | ⬜ |
| T7-02 | Propagação ponta a ponta de trace id | P-INT-4 | ⬜ |
| T7-03 | Convivência de versões de contrato | P-INT-5 | ⬜ |
| T7-04 | Validação zero-trust de entradas de SOFIA | P-INT-6 | ⬜ |
| T7-05 | Garantir operação autônoma sem SOFIA | C-04 | ⬜ |
| T7-06 | Homologação da integração | M6 | ⬜ |

---

## Histórico de Revisões

| Versão | Data | Autor | Descrição |
|--------|------|-------|-----------|
| 0.1.0 | 2026-10-22 | Marcus Oliveira | Backlog inicial de tarefas (SDD). |
