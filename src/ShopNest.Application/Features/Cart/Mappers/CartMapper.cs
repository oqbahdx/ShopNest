using ShopNest.Application.Features.Cart.DTOs;
using CartEntity = ShopNest.Domain.Entities.Cart;
using CartItemEntity = ShopNest.Domain.Entities.CartItem;

namespace ShopNest.Application.Features.Cart.Mappers;

/// <summary>
/// Centralised mapper so every handler produces an identical CartDto.
/// </summary>
public static class CartMapper
{
    public static CartDto ToDto(CartEntity cart) => new(
        Id:                cart.Id,
        UserId:            cart.UserId,
        Items:             cart.Items.Select(ItemToDto).ToList(),
        AppliedCouponCode: cart.Coupon?.Code,
        DiscountAmount:    cart.DiscountAmount,
        SubTotal:          cart.SubTotal,
        Total:             cart.Total
    );

    private static CartItemDto ItemToDto(CartItemEntity item) => new(
        Id:              item.Id,
        ProductId:       item.ProductId,
        ProductName:     item.Product.Name,
        ProductSlug:     item.Product.Slug,
        PrimaryImageUrl: item.Product.Images
            .FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
        UnitPrice:       item.UnitPrice,
        Quantity:        item.Quantity,
        LineTotal:       item.LineTotal,
        IsInStock:       item.Product.StockQuantity > 0
    );
}