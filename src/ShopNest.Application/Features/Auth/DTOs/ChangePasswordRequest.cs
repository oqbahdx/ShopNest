namespace ShopNest.Application.Features.Auth.DTOs;
public sealed record ChangePasswordRequest(
    string CurrentPassword, string NewPassword, string ConfirmNewPassword);
