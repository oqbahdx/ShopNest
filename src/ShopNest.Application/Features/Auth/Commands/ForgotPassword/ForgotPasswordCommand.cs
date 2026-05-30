using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
