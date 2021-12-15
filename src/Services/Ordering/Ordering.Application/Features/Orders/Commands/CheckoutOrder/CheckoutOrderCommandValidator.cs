using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
{
    public CheckoutOrderCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty().WithMessage("{Username} is required")
            .MaximumLength(50).WithMessage("{Username} must not exceed 50 characters");

        RuleFor(c => c.EmailAddress)
            .NotEmpty().WithMessage("{Email} is required");

        RuleFor(c => c.TotalPrice)
            .NotEmpty().WithMessage("{TotalPrice} is required")
            .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero");
    }
}