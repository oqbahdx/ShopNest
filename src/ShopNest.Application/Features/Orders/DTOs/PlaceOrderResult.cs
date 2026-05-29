namespace ShopNest.Application.Features.DTOs;

/// <summary>
/// Minimal result returned after a successful PlaceOrder.
/// The controller uses OrderId to redirect to the payment flow (Phase 4).
/// </summary>
public sealed record PlaceOrderResult(
    Guid    OrderId,
    string  OrderNumber,
    decimal TotalAmount
);