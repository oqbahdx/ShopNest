using Microsoft.AspNetCore.Identity;
using Serilog;
using Scalar.AspNetCore;
using ShopNest.API.Extensions;
using ShopNest.API.Middleware;
using ShopNest.Application;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Infrastructure;
using ShopNest.Infrastructure.Identity;
using ShopNest.Infrastructure.Services;
using ShopNest.Infrastructure.Settings;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ── Serilog ───────────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .Enrich.WithMachineName()
       .WriteTo.Console(outputTemplate:
           "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
       .WriteTo.File("logs/shopnest-.log",
           rollingInterval: RollingInterval.Day,
           retainedFileCountLimit: 30));

// ── Application services ──────────────────────────────────────────────────────
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCatalogInfrastructure();

// ── Stripe ────────────────────────────────────────────────────────────────────
builder.Services.Configure<ShopNest.Application.Common.Settings.StripeSettings>(
    builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<IPaymentService, StripePaymentService>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = long.MaxValue;
});

// ── Redis ─────────────────────────────────────────────────────────────────────
builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection(RedisSettings.SectionName));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(
        builder.Configuration
            .GetSection(RedisSettings.SectionName)
            .GetValue<string>("ConnectionString")
        ?? "localhost:6379"));

// ── Cloudflare R2 ─────────────────────────────────────────────────────────────
builder.Services.Configure<CloudflareR2Settings>(
    builder.Configuration.GetSection(CloudflareR2Settings.SectionName));

// ── Rate limiting ─────────────────────────────────────────────────────────────
builder.Services.AddShopNestRateLimiting();

// ── Controllers + OpenAPI ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// ── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:3000", "https://app.shopnest.com")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()));

var app = builder.Build();

// ── Seed roles ────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILogger<Program>>();
    await RoleSeeder.SeedAsync(roleManager, logger);
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

// Rate limiting must be before authentication
app.UseShopNestRateLimiting();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();

public partial class Program { }
