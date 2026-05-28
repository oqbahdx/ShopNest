using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateStock;

public sealed record UpdateStockCommand(Guid ProductId, int Quantity, string? Reason) : IRequest<Result>;
