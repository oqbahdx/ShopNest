using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ShopNest.Application.Common.Interfaces;

namespace ShopNest.Infrastructure.Services;

public sealed class LocalFileService : IFileService
{
	private readonly IWebHostEnvironment _env;

	private readonly IHttpContextAccessor _httpContext;

	public LocalFileService(IWebHostEnvironment env, IHttpContextAccessor httpContext)
	{
		_env = env;
		_httpContext = httpContext;
	}

	public async Task<FileUploadResult> UploadAsync(IFormFile file, string container, CancellationToken ct = default(CancellationToken))
	{
		string uploadsDir = Path.Combine(_env.WebRootPath, "uploads", container);
		Directory.CreateDirectory(uploadsDir);
		string ext = Path.GetExtension(file.FileName);
		string fileName = $"{Guid.NewGuid()}{ext}";
		string filePath = Path.Combine(uploadsDir, fileName);
		FileUploadResult result;
		await using (FileStream stream = File.Create(filePath))
		{
			await file.CopyToAsync(stream, ct);
			HttpRequest request = _httpContext.HttpContext.Request;
			string baseUrl = $"{request.Scheme}://{request.Host}";
			string url = $"{baseUrl}/uploads/{container}/{fileName}";
			result = new FileUploadResult(url, fileName, file.Length);
		}
		return result;
	}

	public Task DeleteAsync(string fileUrl, string container, CancellationToken ct = default(CancellationToken))
	{
		try
		{
			string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
			string path = Path.Combine(_env.WebRootPath, "uploads", container, fileName);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch
		{
		}
		return Task.CompletedTask;
	}
}
