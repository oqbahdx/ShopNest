using System;
using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Payment : AuditableEntity
{
	public Guid OrderId { get; set; }

	public decimal Amount { get; set; }

	public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

	public PaymentMethod Method { get; set; } = PaymentMethod.Card;

	public string? StripePaymentIntentId { get; set; }

	public string? StripeChargeId { get; set; }

	public string? StripeCustomerId { get; set; }

	public string? FailureReason { get; set; }

	public DateTime? PaidAt { get; set; }

	public decimal RefundedAmount { get; set; } = default(decimal);

	public DateTime? RefundedAt { get; set; }

	public string? RefundReason { get; set; }

		public Order Order { get; set; } = null!;
	public string Currency { get; set; } = string.Empty;

	public static Payment Create(
		Guid orderId,
		string stripePaymentIntentId,
		decimal amount,
		string currency)
	{
		return new Payment
		{
			OrderId = orderId,
			StripePaymentIntentId = stripePaymentIntentId,
			Amount = amount,
			Currency = currency,
			Status = PaymentStatus.Pending,
			Method = PaymentMethod.Card
		};
	}

	public void UpdatePaymentIntentId(string stripePaymentIntentId)
	{
		StripePaymentIntentId = stripePaymentIntentId;
	}

	public void MarkSucceeded(string chargeId)
	{
		Status = PaymentStatus.Succeeded;
		StripeChargeId = chargeId;
		PaidAt = DateTime.UtcNow;
	}

	public void MarkFailed(string reason)
	{
		Status = PaymentStatus.Failed;
		FailureReason = reason;
	}

	public void ApplyRefund(decimal refundAmount, string? reason = null)
	{
		if (refundAmount <= 0m)
		{
			throw new ArgumentException("Refund amount must be positive.");
		}
		if (RefundedAmount + refundAmount > Amount)
		{
			throw new InvalidOperationException("Refund amount exceeds the original payment amount.");
		}
		RefundedAmount += refundAmount;
		RefundReason = reason;
		RefundedAt = DateTime.UtcNow;
		Status = ((RefundedAmount >= Amount) ? PaymentStatus.Refunded : PaymentStatus.PartialRefunded);
	}
}
