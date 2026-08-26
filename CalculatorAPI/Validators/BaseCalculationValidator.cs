using FluentValidation;
using CalculatorAPI.Models;

namespace CalculatorAPI.Validators;

public class BaseCalculationValidator<T> : AbstractValidator<T> where T : ICalculation
{
    public BaseCalculationValidator()
    {
        RuleFor(x => x.Operator)
            .Must(op => new[] { "+", "-", "*", "/", "%" }.Contains(op))
            .WithMessage("O operador é inválido.")
            .When(x => !string.IsNullOrEmpty(x.Operator));

        // Aplica a regra apenas se o operando tiver valor
        RuleFor(x => x.LeftOperand)
            .InclusiveBetween(-999999999m, 999999999m)
            .When(x => x.LeftOperand.HasValue);

        RuleFor(x => x.RightOperand)
            .InclusiveBetween(-999999999m, 999999999m)
            .When(x => x.RightOperand.HasValue);
    }
}