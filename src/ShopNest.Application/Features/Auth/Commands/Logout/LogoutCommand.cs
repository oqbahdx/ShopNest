using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string RawRefreshToken, string? IpAddress)
    : IRequest<Result>;
