namespace CalculatorAPI.Services;

public class CalculatorService
{
    // Esse método realiza o cálculo com base nos operandos e operador fornecidos
    public decimal Calculate(decimal leftOperand, string operatorSymbol, decimal rightOperand)
    {
        return operatorSymbol switch
        {
            "+" => leftOperand + rightOperand,
            "-" => leftOperand - rightOperand,
            "*" => leftOperand * rightOperand,
            "/" => rightOperand != 0 ? leftOperand / rightOperand : throw new DivideByZeroException("Não é possível dividir por zero."),
            _ => throw new ArgumentException($"Operador inválido: {operatorSymbol}") // Default case para operadores inválidos
        };
    }
}