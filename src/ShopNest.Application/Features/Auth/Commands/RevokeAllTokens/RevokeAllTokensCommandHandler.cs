using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.RevokeAllTokens;

public sealed class RevokeAllTokensCommandHandler
    : IRequestHandler<RevokeAllTokensCommand, Result>
{
    private readonly IAppDbContext                             _db;
    private readonly ILogger<RevokeAllTokensCommandHandler>   _logger;

    public RevokeAllTokensCommandHandler(IAppDbContext db,
        ILogger<RevokeAllTokensCommandHandler> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task<Result> Handle(RevokeAllTokensCommand request, CancellationToken ct)
    {
        var count = await _db.RefreshTokens
            .Where(t => t.UserId == request.UserId && !t.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.IsRevoked,   true)
                .SetProperty(t => t.RevokedAt,   DateTime.UtcNow)
                .SetProperty(t => t.RevokedByIp, request.IpAddress), ct);

        _logger.LogInformation(
            "Revoked {Count} active session(s) for user {UserId} from {IP}",
            count, request.UserId, request.IpAddress);

        return Result.Success();
    }
}
