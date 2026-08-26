namespace CalculatorAPI.Models;

public record CalculationRequest : ICalculation
{
    public decimal? LeftOperand { get; init; }
    public string? Operator { get; init; }
    public decimal? RightOperand { get; init; }
    public string? Expression { get; init; }
}
