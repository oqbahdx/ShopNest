using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductByIdQuery(Guid Id)
    : IRequest<Result<ProductDto>>;