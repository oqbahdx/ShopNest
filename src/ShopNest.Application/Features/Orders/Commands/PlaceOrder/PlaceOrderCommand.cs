using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Commands;

/// <summary>
/// Phase 3: Inline address snapshot.
/// Phase 5 upgrade: replace fields with ShippingAddressId (Guid)
/// and load from the Address entity.
/// </summary>
public sealed record PlaceOrderCommand(
    string  ShippingFullName,
    string  ShippingLine1,
    string? ShippingLine2,
    string  ShippingCity,
    string  ShippingState,
    string  ShippingPostalCode,
    string  ShippingCountry
) : IRequest<Result<PlaceOrderResult>>;