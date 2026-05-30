using System.Collections.Generic;
using ShopNest.Application.Common.Identity;

namespace ShopNest.Application.Common.Interfaces;

public interface IJwtService
{
	string GenerateAccessToken(AppUser user, IList<string> roles);

	string GenerateRefreshToken();

	string HashRefreshToken(string rawToken);
}
