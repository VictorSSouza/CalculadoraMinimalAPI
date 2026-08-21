using FluentValidation;
using CalculatorAPI.Models;

namespace CalculatorAPI.Validators;

public class BaseCalculationValidator<T> : AbstractValidator<T> where T : ICalculation
{
    public BaseCalculationValidator()
    {
        RuleFor(x => x.Operator)
            .NotEmpty().WithMessage("O operador não pode ser vazio.")
            .Must(op => new[] { "+", "-", "*", "/" }.Contains(op))
            .WithMessage("O operador é inválido.");

        RuleFor(x => x.LeftOperand)
            .InclusiveBetween(-999999999, 999999999);

        RuleFor(x => x.RightOperand)
            .InclusiveBetween(-999999999, 999999999);
    }
}