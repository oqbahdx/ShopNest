using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ShopNest.Infrastructure.Identity;

public static class RoleSeeder
{
	public static readonly string[] Roles = new string[4] { "SuperAdmin", "Admin", "Vendor", "Customer" };

	public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager, ILogger logger)
	{
		string[] roles = Roles;
		foreach (string role in roles)
		{
			if (await roleManager.RoleExistsAsync(role))
			{
				continue;
			}
			IdentityResult result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
			if (result.Succeeded)
			{
				logger.LogInformation("Role '{Role}' created", role);
				continue;
			}
			logger.LogError("Failed to create role '{Role}': {Errors}", role, string.Join(", ", result.Errors.Select((IdentityError e) => e.Description)));
		}
	}
}
