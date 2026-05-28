namespace ShopNest.Application.Common.Models;

public static class ErrorCodes
{
    public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string AccountDeactivated = "AUTH_ACCOUNT_DEACTIVATED";
    public const string EmailNotConfirmed = "AUTH_EMAIL_NOT_CONFIRMED";
    public const string AccountLocked = "AUTH_ACCOUNT_LOCKED";
    public const string EmailAlreadyRegistered = "AUTH_EMAIL_TAKEN";
    public const string InvalidToken = "AUTH_INVALID_TOKEN";
    public const string TokenExpired = "AUTH_TOKEN_EXPIRED";
    public const string SuspectedTokenTheft = "AUTH_SUSPECTED_THEFT";
    public const string PasswordMismatch = "AUTH_PASSWORD_MISMATCH";
    public const string IdentityError = "AUTH_IDENTITY_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Conflict = "CONFLICT";
    public const string Forbidden = "FORBIDDEN";
    public const string InsufficientStock = "INSUFFICIENT_STOCK";

    public const string NOT_FOUND = NotFound;
    public const string VALIDATION_ERROR = ValidationError;
    public const string CONFLICT = Conflict;
    public const string FORBIDDEN = Forbidden;
    public const string INSUFFICIENT_STOCK = InsufficientStock;
}
