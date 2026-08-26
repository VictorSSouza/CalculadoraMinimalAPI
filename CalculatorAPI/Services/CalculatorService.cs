using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

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
            "%" => leftOperand * (rightOperand / 100m),
            _ => throw new ArgumentException($"Operador inválido: {operatorSymbol}") // Default case para operadores inválidos
        };
    }

    // Novo método para avaliar a string da expressão
    public decimal EvaluateExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("A expressão não pode estar vazia.");

        string expr = expression.Trim().Replace(",", ".");

        // Tratamento de porcentagens complexas
        expr = Regex.Replace(expr, @"(\d+(?:\.\d+)?)\s*([\+\-])\s*(\d+(?:\.\d+)?)%", "$1 $2 ($1 * ($3 / 100.0))");
        expr = Regex.Replace(expr, @"(\d+(?:\.\d+)?)%", "($1 / 100.0)");
        expr = Regex.Replace(expr, @"(\([^\)]+\))%", "($1 / 100.0)");

        // Multiplicação implícita com parênteses
        expr = Regex.Replace(expr, @"(\d)\s*\(", "$1*(");
        expr = Regex.Replace(expr, @"\)\s*(\d)", ")*$1");
        expr = Regex.Replace(expr, @"\)\s*\(", ")*(");

        try
        {
            using var table = new DataTable();
            var rawResult = table.Compute(expr, null);

            if (rawResult == DBNull.Value || rawResult == null)
                throw new InvalidOperationException("Expressão matemática inválida.");

            double doubleResult = Convert.ToDouble(rawResult, CultureInfo.InvariantCulture);

            if (double.IsInfinity(doubleResult) || double.IsNaN(doubleResult))
                throw new DivideByZeroException("Não é possível dividir por zero.");

            return Convert.ToDecimal(doubleResult);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao avaliar expressão: {ex.Message}");
        }
    }
}