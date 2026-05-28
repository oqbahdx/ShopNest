using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string  Email,
    string  Password,
    string? IpAddress
) : IRequest<Result<AuthTokenPair>>;
