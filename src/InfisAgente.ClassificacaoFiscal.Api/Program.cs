// Bootstrap da API do Agente de Classificacao Fiscal de Produtos.
// Fase de estrutura da solucao: sem endpoints nem logica de negocio.
// Endpoints e funcionalidades seguem o fluxo SDD (ver specs/plan.md e specs/tasks.md).
var builder = WebApplication.CreateBuilder(args);

// Documentacao de API (Swagger/OpenAPI) — RF-08, P-INT-1, P-INT-2.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
