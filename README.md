# Agente de Classificação Fiscal de Produtos

Agente de IA da **INFIS Consultoria** para classificação fiscal de produtos (NCM, CEST e atributos tributários correlatos), com rastreabilidade, fundamentação legal e grau de confiança explícitos.

- **Responsável:** Marcus Oliveira
- **Orquestrador central:** SOFIA (integração futura)
- **Fase atual:** Especificação SDD (Spec-Driven Development) — **sem implementação de código**

## Stack-alvo

- **.NET 10 — ASP.NET Core Web API**
- **OpenAI** (LLM: chat + embeddings)
- **RAG:** PostgreSQL + **PgVector**
- **Memória conversacional:** **MongoDB**
- **Orquestração de IA:** Microsoft **Semantic Kernel**
- **Documentação de API:** **Swagger/OpenAPI**
- **Testes:** **xUnit**

## Documentação (SDD)

| Documento | Conteúdo |
|-----------|----------|
| [specs/constitution.md](./specs/constitution.md) | Princípios inegociáveis (isolamento, auditoria, segurança, integração multi-agente). |
| [specs/requirements.md](./specs/requirements.md) | Requisitos funcionais e não funcionais. |
| [specs/architecture.md](./specs/architecture.md) | Arquitetura-alvo, stack e contrato. |
| [specs/plan.md](./specs/plan.md) | Fases e marcos. |
| [specs/tasks.md](./specs/tasks.md) | Backlog de tarefas. |
| [AGENTS.md](./AGENTS.md) | Guia operacional para humanos e agentes de IA. |
