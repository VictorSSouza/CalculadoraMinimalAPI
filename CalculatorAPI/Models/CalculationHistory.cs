using System.ComponentModel.DataAnnotations;

namespace CalculatorAPI.Models;

public class CalculationHistory : ICalculation
{
    [Key]
    public int Id { get; set; } // Identificador único do histórico de cálculo
    [Required]
    public decimal LeftOperand { get; set; } // Operando esquerdo da operação
    public string Operator { get; set; } = string.Empty; // Operador da operação (ex: +, -, *, /)
    public decimal RightOperand { get; set; } // Operando direito da operação
    [Required]
    public decimal Result { get; set; } // Resultado do cálculo
    public DateTime CreatedAt { get; set; } // Data e hora em que o cálculo foi realizado

}