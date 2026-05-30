using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShopNest.Application.Common.Interfaces;

namespace ShopNest.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
	private static class JwtClaimNames
	{
		public const string Email = "email";
	}

	private readonly IHttpContextAccessor _http;

	private ClaimsPrincipal? Principal => _http.HttpContext?.User;

	public Guid? UserId
	{
		get
		{
				string? input = Principal?.FindFirstValue("uid") ?? Principal?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
			Guid result;
			return Guid.TryParse(input, out result) ? new Guid?(result) : ((Guid?)null);
		}
	}

	public string? Email => Principal?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress") ?? Principal?.FindFirstValue("email");

	public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

	public string? IpAddress => _http.HttpContext?.Connection.RemoteIpAddress?.ToString();

	public CurrentUserService(IHttpContextAccessor http)
	{
		_http = http;
	}
}
