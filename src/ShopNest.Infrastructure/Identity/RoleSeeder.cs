using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ShopNest.Infrastructure.Identity;

/// <summary>
/// Seeds the four application roles on startup if they do not yet exist.
/// Call from Program.cs after app.Build().
/// </summary>
public static class RoleSeeder
{
    public static readonly string[] Roles =
    {
        "SuperAdmin",
        "Admin",
        "Vendor",
        "Customer"
    };

    public static async Task SeedAsync(
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger                         logger)
    {
        foreach (var role in Roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            if (result.Succeeded)
                logger.LogInformation("Role '{Role}' created", role);
            else
                logger.LogError("Failed to create role '{Role}': {Errors}",
                    role, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
