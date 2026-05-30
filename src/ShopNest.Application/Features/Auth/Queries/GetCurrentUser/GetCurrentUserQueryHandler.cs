using MediatR;
using Microsoft.AspNetCore.Identity;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Auth.DTOs;

namespace ShopNest.Application.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetCurrentUserQueryHandler(UserManager<AppUser> userManager)
        => _userManager = userManager;

    public async Task<Result<CurrentUserDto>> Handle(
        GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result<CurrentUserDto>.Failure("User not found.", ErrorCodes.NotFound);

        var roles = await _userManager.GetRolesAsync(user);

        return Result<CurrentUserDto>.Success(new CurrentUserDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email!,
            user.AvatarUrl,
            roles.ToList().AsReadOnly()));
    }
}
