namespace ShopNest.Application.Common.Interfaces;

public interface IFileService
{
    Task DeleteAsync(string path, string? container, CancellationToken cancellationToken = default);
}
