using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Settings;

namespace ShopNest.Infrastructure.Services;

public sealed class JwtService : IJwtService
{
	private readonly JwtSettings _settings;

	public JwtService(IOptions<JwtSettings> options)
	{
		_settings = options.Value;
	}

	public string GenerateAccessToken(AppUser user, IList<string> roles)
	{
		List<Claim> list = new List<Claim>
		{
			new Claim("sub", user.Id.ToString()),
			new Claim("email", user.Email ?? string.Empty),
			new Claim("jti", Guid.NewGuid().ToString()),
			new Claim("given_name", user.FirstName),
			new Claim("family_name", user.LastName),
			new Claim("uid", user.Id.ToString())
		};
		foreach (string role in roles)
		{
			list.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
		}
		SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
		SigningCredentials signingCredentials = new SigningCredentials(key, "HS256");
		JwtSecurityToken token = new JwtSecurityToken(_settings.Issuer, _settings.Audience, list, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes), signingCredentials);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		byte[] array = new byte[64];
		RandomNumberGenerator.Fill(array);
		return Convert.ToBase64String(array);
	}

	public string HashRefreshToken(string rawToken)
	{
		byte[] inArray = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
		return Convert.ToHexString(inArray).ToLowerInvariant();
	}
}
