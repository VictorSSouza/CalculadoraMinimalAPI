using FluentValidation;
using CalculatorAPI.Models;
using CalculatorAPI.Validators;
using CalculatorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<CalculationRequestValidator>();
builder.Services.AddTransient<IValidator<CalculationRequest>, CalculationRequestValidator>();

builder.Services.AddTransient<CalculatorService>();

builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

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

app.MapPost("/calcular", async (CalculationRequest request, IValidator<CalculationRequest> validator, CalculatorService calculatorService) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(
        validationResult.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()
        ));
    }

    try
    {
        // Chama o serviço de cálculo para realizar a operação
        var result = calculatorService.Calculate(request.LeftOperand, request.Operator, request.RightOperand);
        // Cria a expressão completa do cálculo como uma string para exibição
        var expression = $"{request.LeftOperand} {request.Operator} {request.RightOperand} = {result}";

        return Results.Ok(new { result, expression });
    }
    catch (DivideByZeroException)
    {
        // Mesmo formato do ValidationProblem
        return Results.ValidationProblem(
            new Dictionary<string, string[]> {
                { "RightOperand", new[] { "Não é possível dividir por zero." } }
            }
        );
    }
    catch (ArgumentException)
    {
        // Mesmo formato do ValidationProblem
        return Results.ValidationProblem(
            new Dictionary<string, string[]> {
                { "Operator", new[] { "Operador inválido." } }
            }
        );
    }
});

app.Run();
