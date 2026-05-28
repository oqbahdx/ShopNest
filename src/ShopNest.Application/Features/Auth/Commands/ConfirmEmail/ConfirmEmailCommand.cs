using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string UserId, string Token)
    : IRequest<Result>;
