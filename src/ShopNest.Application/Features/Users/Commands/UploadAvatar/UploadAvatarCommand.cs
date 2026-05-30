using MediatR;
using Microsoft.AspNetCore.Http;

namespace ShopNest.Application.Features.Users.Commands.UploadAvatar;

public sealed record UploadAvatarCommand(IFormFile File)
    : IRequest<Result<string>>;