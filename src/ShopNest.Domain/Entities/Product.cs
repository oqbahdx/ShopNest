using System;
using System.Collections.Generic;
using System.Linq;
using ShopNest.Domain.Entities.Common;

namespace ShopNest.Domain.Entities;

public class Product : AuditableEntity, ISoftDeletable
{
	public string Name { get; set; } = string.Empty;

	public string Slug { get; set; } = string.Empty;

	public string? Description { get; set; }

	public string? ShortDescription { get; set; }

	public decimal Price { get; set; }

	public decimal? CompareAtPrice { get; set; }

	public decimal? CostPrice { get; set; }

	public int StockQuantity { get; set; } = 0;

	public int LowStockThreshold { get; set; } = 5;

	public string SKU { get; set; } = string.Empty;

	public string? Barcode { get; set; }

	public decimal? Weight { get; set; }

	public Guid CategoryId { get; set; }

	public bool IsFeatured { get; set; } = false;

	public bool IsActive { get; set; } = true;

	public decimal AverageRating { get; set; } = default(decimal);

	public int ReviewCount { get; set; } = 0;

	public bool IsDeleted { get; set; } = false;

	public DateTime? DeletedAt { get; set; }

	public Guid? DeletedBy { get; set; }

		public Category Category { get; set; } = null!;

	public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

	public ICollection<Review> Reviews { get; set; } = new List<Review>();

	public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

	public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

	public bool IsInStock()
	{
		return StockQuantity > 0;
	}

	public bool IsLowStock()
	{
		return StockQuantity > 0 && StockQuantity <= LowStockThreshold;
	}

	public static Product Create(string name, string? description, string? shortDescription, string sku, string slug, decimal price, decimal? compareAtPrice, decimal? costPrice, decimal? weight, int stockQuantity, int lowStockThreshold, bool isFeatured, Guid categoryId)
	{
		return new Product
		{
			Name = name,
			Description = description,
			ShortDescription = shortDescription,
			SKU = sku,
			Slug = slug,
			Price = price,
			CompareAtPrice = compareAtPrice,
			CostPrice = costPrice,
			Weight = weight,
			StockQuantity = stockQuantity,
			LowStockThreshold = lowStockThreshold,
			IsFeatured = isFeatured,
			CategoryId = categoryId
		};
	}

	public void Update(string name, string? description, string? shortDescription, string sku, string slug, decimal price, decimal? compareAtPrice, decimal? costPrice, decimal? weight, int lowStockThreshold, bool isFeatured, bool isActive, Guid categoryId)
	{
		Name = name;
		Description = description;
		ShortDescription = shortDescription;
		SKU = sku;
		Slug = slug;
		Price = price;
		CompareAtPrice = compareAtPrice;
		CostPrice = costPrice;
		Weight = weight;
		LowStockThreshold = lowStockThreshold;
		IsFeatured = isFeatured;
		IsActive = isActive;
		CategoryId = categoryId;
	}

	public void SetStock(int quantity)
	{
		if (quantity < 0)
		{
			throw new ArgumentException("Quantity cannot be negative.", "quantity");
		}
		StockQuantity = quantity;
	}

	public void DecrementStock(int quantity)
	{
		if (quantity <= 0)
		{
			throw new ArgumentException("Quantity must be positive.", "quantity");
		}
		if (StockQuantity < quantity)
		{
			throw new InvalidOperationException("Insufficient stock for product '" + Name + "'.");
		}
		StockQuantity -= quantity;
	}

	public void IncrementStock(int quantity)
	{
		if (quantity <= 0)
		{
			throw new ArgumentException("Quantity must be positive.", "quantity");
		}
		StockQuantity += quantity;
	}

	public void RecalculateRating(IEnumerable<Review> approvedReviews)
	{
		List<Review> list = approvedReviews.ToList();
		ReviewCount = list.Count;
		AverageRating = ((list.Count == 0) ? 0m : Math.Round((decimal)list.Average((Review r) => r.Rating), 2));
	}
}
