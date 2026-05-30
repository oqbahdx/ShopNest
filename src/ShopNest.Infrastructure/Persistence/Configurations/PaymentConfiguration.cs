using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> b)
	{
		b.HasKey((Payment p) => p.Id);
		b.Property((Payment p) => p.Amount).HasColumnType("decimal(18,2)");
		b.Property((Payment p) => p.RefundedAmount).HasColumnType("decimal(18,2)");
		b.Property((Payment p) => p.StripePaymentIntentId).HasMaxLength(200);
		b.Property((Payment p) => p.StripeChargeId).HasMaxLength(200);
		b.Property((Payment p) => p.StripeCustomerId).HasMaxLength(200);
		b.Property((Payment p) => p.FailureReason).HasMaxLength(500);
		b.Property((Payment p) => p.RefundReason).HasMaxLength(500);
		b.HasIndex((Payment p) => p.StripePaymentIntentId);
		b.HasIndex((Payment p) => p.OrderId).IsUnique();
	}
}
