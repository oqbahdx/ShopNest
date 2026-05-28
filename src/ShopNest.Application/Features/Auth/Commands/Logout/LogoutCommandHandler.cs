using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IApplicationDbContext              _db;
    private readonly IJwtService               _jwtService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(IApplicationDbContext db, IJwtService jwtService,
        ILogger<LogoutCommandHandler> logger)
    {
        _db         = db;
        _jwtService = jwtService;
        _logger     = logger;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RawRefreshToken))
            return Result.Success(); // nothing to revoke

        var hash  = _jwtService.HashRefreshToken(request.RawRefreshToken);
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (token is null || token.IsRevoked)
            return Result.Success(); // idempotent — already logged out

        token.Revoke(request.IpAddress);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} logged out from {IP}",
            token.UserId, request.IpAddress);

        return Result.Success();
    }
}
