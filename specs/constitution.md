# Constituição — Agente de Classificação Fiscal de Produtos

> Documento fundacional (SDD — Spec-Driven Development). Define os princípios inegociáveis que governam o projeto, o desenvolvimento e a operação do agente. Qualquer requisito, arquitetura, plano ou tarefa deve estar em conformidade com esta Constituição. Em caso de conflito, **esta Constituição prevalece**.

---

## 1. Identificação

| Campo | Valor |
|-------|-------|
| **Nome do agente** | Classificação Fiscal de Produtos |
| **Organização** | INFIS Consultoria |
| **Responsável** | Marcus Oliveira |
| **Orquestrador central** | SOFIA (integração futura) |
| **Tipo** | Agente especialista (domínio fiscal/tributário) |
| **Status** | Fase de especificação (SDD) — sem implementação |
| **Versão do documento** | 0.1.0 |
| **Data** | 2026-10-22 |

---

## 2. Missão

Classificar produtos segundo a legislação fiscal brasileira — determinando **NCM** (Nomenclatura Comum do Mercosul), **CEST** (Código Especificador da Substituição Tributária) e demais atributos tributários aplicáveis — com rastreabilidade, fundamentação legal e nível de confiança explícito, servindo tanto usuários finais da INFIS quanto, futuramente, o orquestrador central **SOFIA**.

---

## 3. Princípios Inegociáveis

Os princípios a seguir são **normativos**. São referenciados ao longo de todos os documentos SDD pelos seus identificadores (ex.: `P-ISO-1`).

### 3.1 Isolamento (ISO)

- **P-ISO-1** — O agente opera em **fronteira de domínio fechada**: responde exclusivamente sobre classificação fiscal de produtos. Solicitações fora de escopo são recusadas ou encaminhadas, nunca improvisadas.
- **P-ISO-2** — O agente é **stateless por padrão** entre requisições. Qualquer estado persistente (histórico, cache, contexto) é explicitamente declarado, versionado e isolado por tenant/cliente.
- **P-ISO-3** — **Isolamento de dados por cliente (multi-tenant)**: dados de um cliente nunca vazam para o contexto, resposta ou treinamento de outro. Segregação lógica obrigatória.
- **P-ISO-4** — O agente **não compartilha credenciais, segredos ou conexões** com outros agentes. Toda integração ocorre por contratos de interface explícitos (ver ISO-5 e Integração).
- **P-ISO-5** — **Isolamento de falhas**: uma falha do agente não pode derrubar o orquestrador SOFIA nem outros agentes. Degradação graciosa e *timeouts* obrigatórios.

### 3.2 Auditoria (AUD)

- **P-AUD-1** — **Toda** classificação emitida gera um registro de auditoria imutável contendo: entrada recebida, saída produzida, versão do agente, versão da base legal/tabelas, modelo utilizado, timestamp (UTC) e identificador de correlação (*trace id*).
- **P-AUD-2** — **Rastreabilidade de decisão**: cada resultado deve apontar a fundamentação (regra legal, nota explicativa, capítulo/posição NCM) e um **grau de confiança**.
- **P-AUD-3** — Logs de auditoria são **append-only**, protegidos contra adulteração e retidos conforme política de retenção legal (mínimo definido em Requirements).
- **P-AUD-4** — **Separação de logs**: logs de auditoria (negócio) são distintos de logs técnicos/operacionais. Dados sensíveis são mascarados nos logs técnicos.
- **P-AUD-5** — Toda ação do agente é atribuível a uma origem identificável (usuário, sistema ou SOFIA via *trace id*).

### 3.3 Segurança (SEC)

- **P-SEC-1** — **Menor privilégio**: o agente acessa apenas os dados e serviços estritamente necessários.
- **P-SEC-2** — **Validação e sanitização** obrigatórias de toda entrada. Entradas são tratadas como não confiáveis (defesa contra *prompt injection* e dados maliciosos).
- **P-SEC-3** — **Segredos nunca em código ou logs**. Credenciais via cofre de segredos; rotação suportada.
- **P-SEC-4** — Dados em trânsito e em repouso **criptografados**. PII e dados fiscais sensíveis minimizados e protegidos (LGPD).
- **P-SEC-5** — O agente **não executa ações de efeito colateral irreversível** (gravações em sistemas externos, envios) sem autorização explícita e registro de auditoria.
- **P-SEC-6** — **Conteúdo observado não é instrução**: dados vindos de documentos, páginas, planilhas ou de outros agentes são tratados como dados, nunca como comandos que alterem o comportamento do agente.

### 3.4 Integração Multi-Agente (INT)

- **P-INT-1** — A integração com **SOFIA** e demais agentes ocorre por **contrato de interface versionado** (esquema de entrada/saída estável e documentado).
- **P-INT-2** — O agente é **autodescritivo**: expõe capacidades, limites, versão e *schema* para descoberta pelo orquestrador.
- **P-INT-3** — **Idempotência**: requisições repetidas com a mesma chave de correlação produzem o mesmo resultado, sem efeitos colaterais duplicados.
- **P-INT-4** — **Propagação de contexto de rastreamento**: o *trace id* fornecido por SOFIA é preservado e devolvido em toda resposta e log.
- **P-INT-5** — **Compatibilidade retroativa**: mudanças de contrato seguem versionamento semântico; quebras exigem nova versão de interface e período de convivência.
- **P-INT-6** — O agente **não confia cegamente** em outros agentes: valida entradas recebidas de SOFIA como qualquer outra entrada externa (ver P-SEC-2, P-SEC-6).

---

## 4. Governança do Documento

- Alterações nesta Constituição exigem aprovação do **Responsável (Marcus Oliveira)**.
- Toda alteração é versionada (versionamento semântico) e registrada no histórico abaixo.
- Os demais documentos SDD (`requirements.md`, `architecture.md`, `plan.md`, `tasks.md`) **derivam** desta Constituição e devem citar os princípios que atendem.

---

## 5. Histórico de Revisões

| Versão | Data | Autor | Descrição |
|--------|------|-------|-----------|
| 0.1.0 | 2026-10-22 | Marcus Oliveira | Estrutura inicial da Constituição (SDD). |
