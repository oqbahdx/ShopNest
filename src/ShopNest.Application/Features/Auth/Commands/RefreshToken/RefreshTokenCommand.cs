using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Auth.Commands.Login;

namespace ShopNest.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string  RawRefreshToken,
    string? IpAddress
) : IRequest<Result<AuthTokenPair>>;
