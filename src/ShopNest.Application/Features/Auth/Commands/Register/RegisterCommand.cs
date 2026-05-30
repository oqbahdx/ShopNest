using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<Result<string>>;
