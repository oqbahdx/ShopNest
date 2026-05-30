using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ResendConfirmation;

public sealed record ResendConfirmationCommand(string Email) : IRequest<Result>;
