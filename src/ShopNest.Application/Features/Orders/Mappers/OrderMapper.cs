using ShopNest.Application.Features.DTOs;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Orders.Mappers;

public static class OrderMapper
{
    public static OrderDto ToDto(Order o) => new(
        Id: o.Id,
        OrderNumber: o.OrderNumber,
        Status: o.Status,
        ShippingFullName: o.ShippingAddress?.FullName ?? string.Empty,
        ShippingLine1: o.ShippingAddress?.Street ?? string.Empty,
        ShippingLine2: null,
        ShippingCity: o.ShippingAddress?.City ?? string.Empty,
        ShippingState: o.ShippingAddress?.State ?? string.Empty,
        ShippingPostalCode: o.ShippingAddress?.PostalCode ?? string.Empty,
        ShippingCountry: o.ShippingAddress?.Country ?? string.Empty,
        CouponCode: o.CouponCode,
        SubTotal: o.SubTotal,
        DiscountAmount: o.DiscountAmount,
        ShippingCost: o.ShippingCost,
        TaxAmount: o.TaxAmount,
        TotalAmount: o.TotalAmount,
        TrackingNumber: o.TrackingNumber,
        Items: o.Items.Select(i => new OrderItemDto(
            ProductId: i.ProductId,
            ProductName: i.ProductName,
            ProductSlug: i.Product?.Slug ?? string.Empty,
            ProductImageUrl: i.ProductImageUrl,
            UnitPrice: i.UnitPrice,
            Quantity: i.Quantity,
            LineTotal: i.UnitPrice * i.Quantity
        )).ToList(),
        CreatedAt: o.CreatedAt,
        UpdatedAt: o.UpdatedAt == default ? o.CreatedAt : o.UpdatedAt
    );
}
