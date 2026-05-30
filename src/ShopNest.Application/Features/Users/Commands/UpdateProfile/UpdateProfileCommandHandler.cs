using MediatR;

namespace ShopNest.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler
    : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateProfileCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateProfileCommand cmd, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct);

        if (user is null)
            return Result.Failure("User not found.", ErrorCodes.NOT_FOUND);

        if (cmd.FirstName is not null) user.FirstName = cmd.FirstName;
        if (cmd.LastName is not null) user.LastName = cmd.LastName;
        if (cmd.Phone is not null) user.PhoneNumber = cmd.Phone;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}