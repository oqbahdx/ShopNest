using System;
using System.Linq;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class OrderItem : BaseEntity
{
	public Guid OrderId { get; set; }

	public Guid ProductId { get; set; }

	public string ProductName { get; set; } = string.Empty;

	public string? ProductImageUrl { get; set; }

	public string ProductSKU { get; set; } = string.Empty;

	public decimal UnitPrice { get; set; }

	public int Quantity { get; set; }

	public decimal TotalPrice { get; set; }

		public Order Order { get; set; } = null!;

		public Product Product { get; set; } = null!;

	public static OrderItem FromProduct(Guid orderId, Product product, int quantity)
	{
		if (quantity <= 0)
		{
			throw new ArgumentException("Quantity must be positive.", "quantity");
		}
		return new OrderItem
		{
			OrderId = orderId,
			ProductId = product.Id,
			ProductName = product.Name,
			ProductImageUrl = product.Images.FirstOrDefault((ProductImage i) => i.IsPrimary)?.ImageUrl,
			ProductSKU = product.SKU,
			UnitPrice = product.Price,
			Quantity = quantity,
			TotalPrice = Math.Round(product.Price * (decimal)quantity, 2)
		};
	}
}
