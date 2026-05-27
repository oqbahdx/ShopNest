namespace ShopNest.API.Models.Products;

public sealed record UpdateStockRequest(
    int NewQuantity,
    string Reason
);