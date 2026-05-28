using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Auth.DTOs;

namespace ShopNest.Application.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId)
    : IRequest<Result<CurrentUserDto>>;
