namespace ShopNest.Domain.Enums;

public enum PaymentStatus
{
	Pending,
	Processing,
	Succeeded,
	Failed,
	Refunded,
	PartialRefunded,
	Cancelled
}
