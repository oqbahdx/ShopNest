namespace ShopNest.Domain.Enums;

public enum NotificationType
{
	OrderPlaced,
	OrderConfirmed,
	OrderShipped,
	OrderDelivered,
	OrderCancelled,
	PaymentReceived,
	PaymentFailed,
	LowStock,
	ReviewApproved,
	PasswordChanged,
	General
}
