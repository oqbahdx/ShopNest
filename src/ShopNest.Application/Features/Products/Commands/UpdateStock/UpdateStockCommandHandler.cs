using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateStock;

public sealed class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateStockCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);
        if (product is null)
            return Result.Failure("Product not found.", ErrorCodes.NotFound);

        product.SetStock(request.Quantity);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
