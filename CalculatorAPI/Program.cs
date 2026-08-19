var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<CalculatorAPI.Services.CalculatorService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
