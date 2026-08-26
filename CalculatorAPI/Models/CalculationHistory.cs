using System.ComponentModel.DataAnnotations;

namespace CalculatorAPI.Models;

public class CalculationHistory : ICalculation
{
    [Key]
    public int Id { get; set; } // Identificador único do histórico de cálculo
    public decimal? LeftOperand { get; set; } // Operando esquerdo da operação
    public string? Operator { get; set; } // Operador da operação (ex: +, -, *, /)
    public decimal? RightOperand { get; set; } // Operando direito da operação
    public string? Expression { get; set; }
    [Required]
    public decimal Result { get; set; } // Resultado do cálculo
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Data e hora em que o cálculo foi realizado

}