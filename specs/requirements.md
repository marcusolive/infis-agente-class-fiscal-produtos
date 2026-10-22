# Requisitos — Agente de Classificação Fiscal de Produtos

> Deriva da [Constituição](./constitution.md). Cada requisito referencia os princípios que atende (ex.: `P-AUD-1`). Convenções: **DEVE** (obrigatório), **DEVERIA** (recomendado), **PODE** (opcional).

| Campo | Valor |
|-------|-------|
| **Agente** | Classificação Fiscal de Produtos |
| **Responsável** | Marcus Oliveira |
| **Orquestrador** | SOFIA (integração futura) |
| **Versão** | 0.1.0 |
| **Data** | 2026-10-22 |

---

## 1. Escopo

### 1.1 Dentro do escopo
- Determinação de **NCM** a partir da descrição de um produto e atributos fornecidos.
- Sugestão de **CEST** quando aplicável.
- Indicação de atributos tributários correlatos (ex.: aplicabilidade de Substituição Tributária, observações de IPI/ICMS quando a base permitir).
- Fundamentação legal e **grau de confiança** da classificação.
- Classificação individual e **em lote**.

### 1.2 Fora do escopo (nesta fase)
- Apuração ou cálculo de impostos devidos.
- Emissão de documentos fiscais (NF-e, etc.).
- Consultoria tributária personalizada vinculante.
- Qualquer ação com efeito colateral em sistemas externos (ver `P-SEC-5`).

---

## 2. Requisitos Funcionais (RF)

| ID | Requisito | Princípios |
|----|-----------|------------|
| **RF-01** | O agente **DEVE** receber a descrição de um produto (texto) e atributos opcionais (marca, aplicação, material, NCM candidato) e retornar a classificação NCM. | P-ISO-1 |
| **RF-02** | O agente **DEVE** retornar, quando aplicável, o código **CEST** correspondente ao NCM determinado. | P-ISO-1 |
| **RF-03** | Cada resposta **DEVE** incluir **grau de confiança** (ex.: 0–100% ou alto/médio/baixo) e **fundamentação** (posição/capítulo NCM, nota explicativa, regra aplicada). | P-AUD-2 |
| **RF-04** | O agente **DEVE** suportar classificação **em lote** (múltiplos produtos em uma requisição), preservando correlação item a item. | P-INT-3 |
| **RF-05** | Quando a confiança for baixa ou houver ambiguidade, o agente **DEVE** retornar **candidatos alternativos** ranqueados, em vez de uma resposta única. | P-AUD-2 |
| **RF-06** | O agente **DEVE** recusar, de forma explícita e rastreável, solicitações fora de escopo (ver 1.2). | P-ISO-1 |
| **RF-07** | O agente **DEVE** registrar auditoria de toda classificação (entrada, saída, versões, modelo, timestamp, trace id). | P-AUD-1 |
| **RF-08** | O agente **DEVE** expor um **manifesto de capacidades** (schema de entrada/saída, versão, limites) para descoberta por SOFIA. | P-INT-2 |
| **RF-09** | O agente **DEVE** preservar e devolver o **trace id** recebido de SOFIA ou do chamador. | P-INT-4 |
| **RF-10** | O agente **DEVERIA** permitir consulta à versão das tabelas/base legal usadas em uma classificação. | P-AUD-1 |
| **RF-11** | O agente **DEVE** indicar explicitamente quando a base legal/tabela de referência estiver desatualizada ou indisponível. | P-AUD-2, P-ISO-5 |

---

## 3. Requisitos Não Funcionais (RNF)

### 3.1 Segurança
| ID | Requisito | Princípios |
|----|-----------|------------|
| **RNF-SEC-01** | Toda entrada **DEVE** ser validada e sanitizada; entradas são não confiáveis. | P-SEC-2, P-SEC-6 |
| **RNF-SEC-02** | Segredos **DEVEM** residir em cofre de segredos, nunca em código ou logs. | P-SEC-3 |
| **RNF-SEC-03** | Dados em trânsito e repouso **DEVEM** ser criptografados; PII minimizada (LGPD). | P-SEC-4 |
| **RNF-SEC-04** | O agente **DEVE** operar sob menor privilégio. | P-SEC-1 |
| **RNF-SEC-05** | Nenhuma ação irreversível sem autorização explícita e auditoria. | P-SEC-5 |

### 3.2 Auditoria e Conformidade
| ID | Requisito | Princípios |
|----|-----------|------------|
| **RNF-AUD-01** | Logs de auditoria **DEVEM** ser *append-only* e protegidos contra adulteração. | P-AUD-3 |
| **RNF-AUD-02** | Retenção de auditoria **DEVE** respeitar o mínimo legal (a confirmar com o Responsável; padrão de projeto: 5 anos). | P-AUD-3 |
| **RNF-AUD-03** | Logs técnicos **DEVEM** mascarar dados sensíveis e ser separados dos logs de negócio. | P-AUD-4 |
| **RNF-AUD-04** | Toda ação **DEVE** ser atribuível a uma origem (usuário/sistema/SOFIA). | P-AUD-5 |

### 3.3 Isolamento
| ID | Requisito | Princípios |
|----|-----------|------------|
| **RNF-ISO-01** | Dados **DEVEM** ser isolados por cliente (multi-tenant); sem vazamento entre tenants. | P-ISO-3 |
| **RNF-ISO-02** | O agente **DEVE** ser stateless por padrão; estado persistente é declarado e isolado. | P-ISO-2 |
| **RNF-ISO-03** | Falhas do agente **DEVEM** ser contidas (timeouts, degradação graciosa) sem afetar SOFIA. | P-ISO-5 |

### 3.4 Integração e Interoperabilidade
| ID | Requisito | Princípios |
|----|-----------|------------|
| **RNF-INT-01** | A interface **DEVE** ser versionada (SemVer); quebras exigem nova versão e convivência. | P-INT-5 |
| **RNF-INT-02** | Requisições **DEVEM** ser idempotentes por chave de correlação. | P-INT-3 |
| **RNF-INT-03** | O contrato de entrada/saída **DEVE** ser documentado e estável. | P-INT-1 |

### 3.5 Desempenho e Confiabilidade (metas iniciais, a validar)
| ID | Requisito |
|----|-----------|
| **RNF-PERF-01** | Latência-alvo de classificação individual: **p95 ≤ 5 s**. |
| **RNF-PERF-02** | Disponibilidade-alvo do serviço: **≥ 99,5%**. |
| **RNF-PERF-03** | Lote: processar **≥ 100 itens** por requisição sem degradação crítica. |

### 3.6 Observabilidade
| ID | Requisito |
|----|-----------|
| **RNF-OBS-01** | Métricas de volume, latência, taxa de recusa e distribuição de confiança **DEVEM** ser expostas. |
| **RNF-OBS-02** | Trace id **DEVE** correlacionar logs, métricas e auditoria. |

---

## 4. Restrições

- **C-01** — Nesta fase **não há implementação de código** (apenas especificação SDD).
- **C-02** — A base legal (NCM/CEST) depende de fontes oficiais atualizadas; o agente não inventa códigos.
- **C-03** — Conformidade com **LGPD** é obrigatória.
- **C-04** — A integração com SOFIA é **futura**; o design deve antecipá-la sem bloquear a operação autônoma.

---

## 5. Premissas

- **A-01** — SOFIA fornecerá *trace id* e autenticação de serviço.
- **A-02** — Haverá uma fonte de verdade para tabelas NCM/CEST versionadas.
- **A-03** — Usuários e sistemas chamadores são autenticados antes de alcançar o agente.

---

## 6. Critérios de Aceite (gerais)

- Toda classificação produz saída estruturada com NCM, CEST (quando aplicável), confiança, fundamentação e trace id.
- Toda classificação gera registro de auditoria imutável.
- Solicitações fora de escopo são recusadas de forma rastreável.
- Nenhum segredo aparece em código ou logs.

---

## 7. Histórico de Revisões

| Versão | Data | Autor | Descrição |
|--------|------|-------|-----------|
| 0.1.0 | 2026-10-22 | Marcus Oliveira | Estrutura inicial de requisitos (SDD). |
