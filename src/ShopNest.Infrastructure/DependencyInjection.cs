using System;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Settings;
using ShopNest.Infrastructure.Persistence;
using ShopNest.Infrastructure.Services;
using ShopNest.Infrastructure.Settings;
using JwtSettings = ShopNest.Application.Common.Settings.JwtSettings;

namespace ShopNest.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<JwtSettings>().Bind(configuration.GetSection("JwtSettings")).ValidateDataAnnotations()
			.ValidateOnStart();
		services.AddOptions<EmailSettings>().Bind(configuration.GetSection("EmailSettings")).ValidateDataAnnotations()
			.ValidateOnStart();
		services.AddDbContext<AppDbContext>(delegate(DbContextOptionsBuilder opts)
		{
			opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), delegate(SqlServerDbContextOptionsBuilder sql)
			{
				sql.EnableRetryOnFailure(3);
			});
		});
		services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
		services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
		services.AddIdentity<AppUser, IdentityRole<Guid>>(delegate(IdentityOptions opts)
		{
			opts.Password.RequireDigit = true;
			opts.Password.RequireLowercase = true;
			opts.Password.RequireUppercase = true;
			opts.Password.RequireNonAlphanumeric = true;
			opts.Password.RequiredLength = 8;
			opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15L);
			opts.Lockout.MaxFailedAccessAttempts = 5;
			opts.Lockout.AllowedForNewUsers = true;
			opts.User.RequireUniqueEmail = true;
			opts.SignIn.RequireConfirmedEmail = false;
		}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
		IConfigurationSection jwtSection = configuration.GetSection("JwtSettings");
		string secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
		services.AddAuthentication(delegate(AuthenticationOptions opts)
		{
			opts.DefaultAuthenticateScheme = "Bearer";
			opts.DefaultChallengeScheme = "Bearer";
		}).AddJwtBearer(delegate(JwtBearerOptions opts)
		{
			opts.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
				ValidateIssuer = true,
				ValidIssuer = jwtSection["Issuer"],
				ValidateAudience = true,
				ValidAudience = jwtSection["Audience"],
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};
			opts.Events = new JwtBearerEvents
			{
				OnChallenge = delegate(JwtBearerChallengeContext ctx)
				{
					ctx.HandleResponse();
					ctx.Response.StatusCode = 401;
					ctx.Response.ContentType = "application/json";
					return ctx.Response.WriteAsync("{\"success\":false,\"message\":\"Authentication required.\",\"errorCode\":\"AUTH_REQUIRED\"}");
				},
				OnForbidden = delegate(ForbiddenContext ctx)
				{
					ctx.Response.StatusCode = 403;
					ctx.Response.ContentType = "application/json";
					return ctx.Response.WriteAsync("{\"success\":false,\"message\":\"You do not have permission.\",\"errorCode\":\"FORBIDDEN\"}");
				}
			};
		});
		services.AddScoped<IJwtService, JwtService>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddHttpContextAccessor();
		return services;
	}
}
