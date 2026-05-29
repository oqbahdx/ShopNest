using MediatR;
using ShopNest.Application.Features.DTOs;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Commands;

public sealed class PlaceOrderCommandHandler
    : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResult>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    // Phase 3 defaults — make configurable in Phase 8
    private const decimal ShippingCost = 5.00m;
    private const decimal TaxRate = 0.00m;

    public PlaceOrderCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PlaceOrderResult>> Handle(
        PlaceOrderCommand cmd, CancellationToken ct)
    {
        if (_currentUser.UserId is not Guid userId)
            return Result<PlaceOrderResult>.Failure(
                "Authentication required.", ErrorCodes.FORBIDDEN);

        // ── Step 1: Load cart with all navigations ─────────────────
        var cart = await _db.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is null || !cart.Items.Any())
            return Result<PlaceOrderResult>.Failure(
                "Your cart is empty.", ErrorCodes.CONFLICT);

        // ── Step 2: Begin atomic transaction ───────────────────────
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            // ── Step 3: Re-validate stock for EVERY item ───────────
            // Prevents race conditions between cart validation and checkout.
            foreach (var item in cart.Items)
            {
                var currentStock = await _db.Products
                    .Where(p => p.Id == item.ProductId)
                    .Select(p => p.StockQuantity)
                    .FirstOrDefaultAsync(ct);

                if (currentStock < item.Quantity)
                    return Result<PlaceOrderResult>.Failure(
                        $"'{item.Product.Name}' only has {currentStock} unit(s) available.",
                        ErrorCodes.INSUFFICIENT_STOCK);
            }

            // ── Step 4: Calculate totals ───────────────────────────
            var subTotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);

            var discountAmount = cart.Coupon is not null
                ? cart.Coupon.CalculateDiscount(subTotal)
                : 0m;

            var taxAmount = Math.Round((subTotal - discountAmount) * TaxRate, 2);
            var totalAmount = subTotal - discountAmount + ShippingCost + taxAmount;

            // ── Step 5: Create Address + Order entities ─────────────
            var shippingAddress = new Address
            {
                Id         = Guid.NewGuid(),
                UserId     = userId,
                FullName   = cmd.ShippingFullName,
                Phone      = string.Empty,
                Street     = cmd.ShippingLine1,
                City       = cmd.ShippingCity,
                State      = cmd.ShippingState,
                PostalCode = cmd.ShippingPostalCode,
                Country    = cmd.ShippingCountry,
                IsDefault  = false
            };

            var order = new Order
            {
                Id                = Guid.NewGuid(),
                UserId            = userId,
                OrderNumber       = Order.GenerateOrderNumber(),
                Status            = OrderStatus.Pending,
                ShippingAddressId = shippingAddress.Id,
                ShippingAddress   = shippingAddress,
                CouponId          = cart.CouponId,
                CouponCode        = cart.Coupon?.Code,
                SubTotal          = subTotal,
                DiscountAmount    = discountAmount,
                ShippingCost      = ShippingCost,
                TaxAmount         = taxAmount,
                TotalAmount       = totalAmount
            };

            // ── Step 6: Snapshot each item via FromProduct() ───────
            // OrderItem.FromProduct() captures price, name, image at this
            // exact moment so future product changes don't alter the order.
            foreach (var cartItem in cart.Items)
            {
                var snapshot = OrderItem.FromProduct(
                    order.Id, cartItem.Product, cartItem.Quantity);
                order.Items.Add(snapshot);
            }

            _db.Orders.Add(order);

            // ── Step 7: Decrement stock ────────────────────────────
            // DecrementStock throws DomainException if stock goes negative —
            // safety net in case another transaction committed between steps 3 and 7.
            foreach (var cartItem in cart.Items)
            {
                var product = await _db.Products
                    .FindAsync(new object[] { cartItem.ProductId }, ct);

                product!.DecrementStock(cartItem.Quantity);
            }

            // ── Step 8: Increment coupon usage ─────────────────────
            if (cart.CouponId.HasValue)
            {
                var coupon = await _db.Coupons
                    .FindAsync(new object[] { cart.CouponId.Value }, ct);

                coupon!.IncrementUsage();
            }

            // ── Step 9: Clear the cart ─────────────────────────────
            cart.Clear();

            // ── Step 10: Atomic save + commit ──────────────────────
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Result<PlaceOrderResult>.Success(new PlaceOrderResult(
                OrderId: order.Id,
                OrderNumber: order.OrderNumber,
                TotalAmount: order.TotalAmount
            ));
        }
        catch (DomainException ex)
        {
            await tx.RollbackAsync(ct);
            return Result<PlaceOrderResult>.Failure(
                ex.Message, ErrorCodes.CONFLICT);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw; // Let ExceptionHandlingMiddleware produce RFC 7807
        }
    }
}