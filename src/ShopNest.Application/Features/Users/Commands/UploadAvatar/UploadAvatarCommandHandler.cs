using MediatR;

namespace ShopNest.Application.Features.Users.Commands.UploadAvatar;

public sealed class UploadAvatarCommandHandler
    : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileService _fileService;

    public UploadAvatarCommandHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IFileService fileService)
    {
        _db = db;
        _currentUser = currentUser;
        _fileService = fileService;
    }

    public async Task<Result<string>> Handle(
        UploadAvatarCommand cmd, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct);

        if (user is null)
            return Result<string>.Failure(
                "User not found.", ErrorCodes.NOT_FOUND);

        // Delete previous avatar from storage (best-effort)
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _fileService.DeleteAsync(user.AvatarUrl, "avatars", ct);
        }

        var uploadResult = await _fileService.UploadAsync(
            cmd.File, "avatars", ct);

        user.AvatarUrl = uploadResult.Url;

        await _db.SaveChangesAsync(ct);
        return Result<string>.Success(uploadResult.Url);
    }
}