using System.ComponentModel.DataAnnotations;

namespace CalculatorAPI.Models;

public interface ICalculation
{
    [Required]
    decimal LeftOperand { get; } // Operando esquerdo da operação
    [Required]
    [MaxLength(5)]
    string Operator { get; } // Operador da operação (ex: +, -, *, /)
    [Required]
    decimal RightOperand { get; } // Operando direito da operação
}