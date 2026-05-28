using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteCategoryCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (category is null)
            return Result.Failure("Category not found.", ErrorCodes.NotFound);

        var hasProducts = await _db.Products.AnyAsync(p => p.CategoryId == request.Id && p.IsActive, ct);
        if (hasProducts)
            return Result.Failure("Category has active products.", ErrorCodes.Conflict);

        var hasChildren = await _db.Categories.AnyAsync(c => c.ParentCategoryId == request.Id && c.IsActive, ct);
        if (hasChildren)
            return Result.Failure("Category has child categories.", ErrorCodes.Conflict);

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
