using MediatR;
using ShopNest.Application.Features.Users.DTOs;

namespace ShopNest.Application.Features.Notifications.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<Result<ProfileDto>>;

public sealed class GetProfileQueryHandler
    : IRequestHandler<GetProfileQuery, Result<ProfileDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetProfileQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ProfileDto>> Handle(
        GetProfileQuery _, CancellationToken ct)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct);

        if (user is null)
            return Result<ProfileDto>.Failure(
                "User not found.", ErrorCodes.NOT_FOUND);

        return Result<ProfileDto>.Success(new ProfileDto(
            Id: user.Id,
            Email: user.Email ?? string.Empty,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Phone: user.PhoneNumber,
            AvatarUrl: user.AvatarUrl,
            IsEmailConfirmed: user.EmailConfirmed,
            CreatedAt: user.CreatedAt
        ));
    }
}