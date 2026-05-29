namespace ShopNest.Application.Features.Payments.DTOs;

/// <summary>
/// Returned to the client after creating a PaymentIntent.
/// The client uses ClientSecret with Stripe.js to complete payment
/// on the frontend — the server never touches the card details.
/// </summary>
public sealed record PaymentIntentDto(
    string  ClientSecret,
    decimal Amount,
    string  Currency
);