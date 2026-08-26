using System.ComponentModel.DataAnnotations;

namespace CalculatorAPI.Models;

public interface ICalculation
{
    decimal? LeftOperand { get; } // Operando esquerdo da operação
    [MaxLength(5)]
    string? Operator { get; } // Operador da operação (ex: +, -, *, /)
    decimal? RightOperand { get; } // Operando direito da operação
    string? Expression { get; } // expressão matemática
}