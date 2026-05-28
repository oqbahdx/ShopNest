using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopNest.Domain.Entities;

namespace ShopNest.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        b.Property(p => p.RefundedAmount).HasColumnType("decimal(18,2)");
        b.Property(p => p.StripePaymentIntentId).HasMaxLength(200);
        b.Property(p => p.StripeChargeId).HasMaxLength(200);
        b.Property(p => p.StripeCustomerId).HasMaxLength(200);
        b.Property(p => p.FailureReason).HasMaxLength(500);
        b.Property(p => p.RefundReason).HasMaxLength(500);

        b.HasIndex(p => p.StripePaymentIntentId);
        b.HasIndex(p => p.OrderId).IsUnique();
    }
}
