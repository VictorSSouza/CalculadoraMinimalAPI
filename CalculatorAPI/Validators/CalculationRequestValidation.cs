using FluentValidation;
using CalculatorAPI.Models;

namespace CalculatorAPI.Validators;

public class CalculationRequestValidator : BaseCalculationValidator<CalculationRequest>
{
    public CalculationRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Expression) ||
                       (x.LeftOperand.HasValue && x.RightOperand.HasValue && !string.IsNullOrWhiteSpace(x.Operator)))
            .WithMessage("Informe uma expressão válida ou o conjunto completo de LeftOperand, Operator e RightOperand.");
    }
}