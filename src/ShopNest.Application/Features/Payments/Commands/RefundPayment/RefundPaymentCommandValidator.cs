using FluentValidation;

namespace ShopNest.Application.Features.Payments.RefundPayment;

public sealed class RefundPaymentCommandValidator
    : AbstractValidator<RefundPaymentCommand>
{
    private static readonly HashSet<string> AllowedReasons =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "duplicate", "fraudulent", "requested_by_customer"
        };

    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("Payment ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Refund amount must be greater than zero.");

        RuleFor(x => x.Reason)
            .Must(r => r is null || AllowedReasons.Contains(r))
            .WithMessage(
                "Reason must be one of: duplicate, fraudulent, requested_by_customer.");
    }
}