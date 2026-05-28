using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.RevokeAllTokens;

/// <summary>Revokes every active session for a user (security lockdown / sign out everywhere).</summary>
public sealed record RevokeAllTokensCommand(Guid UserId, string? IpAddress)
    : IRequest<Result>;
