namespace ShopNest.Application.Common.Interfaces;

public interface IFileService
{
    Task<FileUploadResult> UploadAsync(
        IFormFile file,
        string container,
        CancellationToken ct = default);
    Task DeleteAsync(
        string fileUrl,
        string container,
        CancellationToken ct = default);
}
public record FileUploadResult(
    string Url,
    string FileName,
    long SizeInBytes);