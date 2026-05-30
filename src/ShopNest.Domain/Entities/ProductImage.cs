using System;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class ProductImage : BaseEntity
{
	public Guid ProductId { get; set; }

	public string ImageUrl { get; set; } = string.Empty;

	public string? ThumbnailUrl { get; set; }

	public string? AltText { get; set; }

	public int DisplayOrder { get; set; } = 0;

	public bool IsPrimary { get; set; } = false;

		public Product Product { get; set; } = null!;

	public static ProductImage Create(Guid productId, string url, string? altText, int displayOrder, bool isPrimary)
	{
		return new ProductImage
		{
			ProductId = productId,
			ImageUrl = url,
			AltText = altText,
			DisplayOrder = displayOrder,
			IsPrimary = isPrimary
		};
	}

	public void SetPrimary(bool isPrimary)
	{
		IsPrimary = isPrimary;
	}
}
