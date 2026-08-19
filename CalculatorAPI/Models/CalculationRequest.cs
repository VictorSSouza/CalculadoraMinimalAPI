namespace CalculatorAPI.Models;

public record CalculationRequest(decimal LeftOperand, string Operator, decimal RightOperand);
