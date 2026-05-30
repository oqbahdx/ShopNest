using MediatR;

namespace ShopNest.Application.Features.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    string? FirstName,
    string? LastName,
    string? Phone
) : IRequest<Result>;