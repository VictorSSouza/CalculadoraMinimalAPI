using FluentValidation;
using CalculatorAPI.Models;
using CalculatorAPI.Data;
using CalculatorAPI.Validators;
using CalculatorAPI.Services;
using CalculatorAPI.Logging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço de logging personalizado
builder.Services.AddHttpContextAccessor();
// Registra o Provider resolvendo a dependência do IHttpContextAccessor
builder.Services.AddSingleton<ILoggerProvider>(sp =>
{
    var config = new CustomLoggerProviderConfiguration { LogLevel = LogLevel.Information };
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    return new CustomLoggerProvider(config, accessor);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Permite converter números enviados como string e tratar valores nulos
    options.SerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
});

builder.Services.AddValidatorsFromAssemblyContaining<CalculationRequestValidator>();

builder.Services.AddTransient<IValidator<CalculationRequest>, CalculationRequestValidator>();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<CalculatorService>();

builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Garantir que o banco SQLite existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configuração dos Middlewares
app.UseCors();

// Ative o middleware no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // Cria a interface visual do Swagger para testar os endpoints
    app.UseSwaggerUI(c =>
    {
        // Configura o Swagger para ser a página inicial do projeto
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = string.Empty; // Transforma o Swagger na página principal do projeto
    });
}

app.MapPost("/calcular", async (
    CalculationRequest request,
    IValidator<CalculationRequest> reqValidator,
    CalculatorService calculatorService,
    AppDbContext db) =>
{
    var reqValidation = await reqValidator.ValidateAsync(request);
    if (!reqValidation.IsValid)
    {
        return Results.ValidationProblem(reqValidation.ToDictionary());
    }

    try
    {
        // Escolhe um caminho com base nas condições de Expression e armazena em result
        decimal result = !string.IsNullOrWhiteSpace(request.Expression)
            ? calculatorService.EvaluateExpression(request.Expression)
            : calculatorService.Calculate(request.LeftOperand!.Value, request.Operator!, request.RightOperand!.Value);

        var history = new CalculationHistory
        {
            LeftOperand = request.LeftOperand,
            RightOperand = request.RightOperand,
            Operator = request.Operator,
            Expression = request.Expression,
            Result = result,
            CreatedAt = DateTime.UtcNow
        };

        db.CalculationHistory.Add(history);
        await db.SaveChangesAsync();

        return Results.Ok(new { Result = result, HistoryId = history.Id });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status400BadRequest,
            title: "Erro na requisição"
        );
    }
});

// Histórico de cálculos realizados
app.MapGet("/historico", async (AppDbContext db, ILogger<Program> logger) =>
{
    logger.LogInformation("Listando o histórico de cálculos");

    var result = await db.CalculationHistory
        .OrderByDescending(ch => ch.CreatedAt)
        .Select(ch => new
        {
            ch.LeftOperand,
            ch.Operator,
            ch.RightOperand,
            ch.Expression,
            ch.Result,
            ch.CreatedAt
        }) // seleciona todos os campos do histórico de cálculos
        .ToArrayAsync();

    return Results.Ok(result);
});


app.MapDelete("/historico", async (AppDbContext db) =>
{
    // Remove todos os registros do histórico de cálculos
    db.CalculationHistory.RemoveRange(db.CalculationHistory);
    await db.SaveChangesAsync();

    return Results.Ok();
});


app.Run();
