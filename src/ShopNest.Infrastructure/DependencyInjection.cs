using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Settings;
using ShopNest.Infrastructure.Persistence;
using ShopNest.Infrastructure.Services;
using ShopNest.Infrastructure.Settings;

namespace ShopNest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Settings binding (validated at startup) ───────────────────────────
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailSettings>()
            .Bind(configuration.GetSection(EmailSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure(3)));
        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>());

        // ── ASP.NET Core Identity ─────────────────────────────────────────────
        services.AddIdentity<AppUser, IdentityRole<Guid>>(opts =>
        {
            // Password policy
            opts.Password.RequireDigit           = true;
            opts.Password.RequireLowercase       = true;
            opts.Password.RequireUppercase       = true;
            opts.Password.RequireNonAlphanumeric = true;
            opts.Password.RequiredLength         = 8;

            // Account lockout — 5 failures → 15-minute lockout
            opts.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
            opts.Lockout.MaxFailedAccessAttempts = 5;
            opts.Lockout.AllowedForNewUsers      = true;

            // Email must be unique and confirmed before sign-in
            opts.User.RequireUniqueEmail         = true;
            opts.SignIn.RequireConfirmedEmail     = false; // enforced manually in handler
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // ── JWT Authentication ────────────────────────────────────────────────
        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        var secretKey  = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer           = true,
                ValidIssuer              = jwtSection["Issuer"],
                ValidateAudience         = true,
                ValidAudience            = jwtSection["Audience"],
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero,  // strict expiry — no grace period
            };

            // Return 401 body instead of empty response
            opts.Events = new JwtBearerEvents
            {
                OnChallenge = ctx =>
                {
                    ctx.HandleResponse();
                    ctx.Response.StatusCode  = 401;
                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync(
                        """{"success":false,"message":"Authentication required.","errorCode":"AUTH_REQUIRED"}""");
                },
                OnForbidden = ctx =>
                {
                    ctx.Response.StatusCode  = 403;
                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync(
                        """{"success":false,"message":"You do not have permission.","errorCode":"FORBIDDEN"}""");
                }
            };
        });

        // ── Application services ──────────────────────────────────────────────
        services.AddScoped<IJwtService,          JwtService>();
        services.AddScoped<IEmailService,        EmailService>();
        services.AddScoped<ICurrentUserService,  CurrentUserService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
