namespace ShopNest.Domain.Enums;

public enum OrderStatus
{
    Pending        = 0,  // Order placed, awaiting payment
    Confirmed      = 1,  // Payment received
    Processing     = 2,  // Being prepared
    Shipped        = 3,  // Dispatched
    Delivered      = 4,  // Customer received
    Cancelled      = 5,  // Cancelled (terminal)
    ReturnRequested = 6, // Return initiated
    Returned       = 7   // Return completed (terminal)
}

public enum PaymentStatus
{
    Pending    = 0,
    Processing = 1,
    Succeeded  = 2,
    Failed     = 3,
    Refunded   = 4,
    PartialRefunded = 5,
    Cancelled  = 6
}

public enum PaymentMethod
{
    Card         = 0,
    BankTransfer = 1,
    Wallet       = 2,
    CashOnDelivery = 3
}

public enum DiscountType
{
    Percentage   = 0,  // e.g. 10% off
    FixedAmount  = 1,  // e.g. $20 off
    FreeShipping = 2
}

public enum ReviewStatus
{
    Pending  = 0,   // Awaiting admin moderation
    Approved = 1,
    Rejected = 2
}

public enum NotificationType
{
    OrderPlaced       = 0,
    OrderConfirmed    = 1,
    OrderShipped      = 2,
    OrderDelivered    = 3,
    OrderCancelled    = 4,
    PaymentReceived   = 5,
    PaymentFailed     = 6,
    LowStock          = 7,
    ReviewApproved    = 8,
    PasswordChanged   = 9,
    General           = 10
}

public enum UserRole
{
    Customer   = 0,
    Vendor     = 1,
    Admin      = 2,
    SuperAdmin = 3
}
