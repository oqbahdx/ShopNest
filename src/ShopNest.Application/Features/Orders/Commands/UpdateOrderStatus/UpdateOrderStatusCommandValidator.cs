using FluentValidation;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Commands.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandValidator
    : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.TrackingNumber)
            .NotEmpty()
            .When(x => x.NewStatus == OrderStatus.Shipped)
            .WithMessage("Tracking number is required when marking an order as Shipped.");

        RuleFor(x => x.TrackingNumber)
            .MaximumLength(200)
            .When(x => x.TrackingNumber is not null);
    }
}