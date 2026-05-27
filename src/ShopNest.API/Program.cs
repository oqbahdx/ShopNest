using Microsoft.AspNetCore.Identity;
using Serilog;
using Scalar.AspNetCore;
using ShopNest.API.Middleware;
using ShopNest.Application;
using ShopNest.Infrastructure;
using ShopNest.Infrastructure.Identity;

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
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB total
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();



// ── CORS (adjust origins for production) ─────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:3000", "https://app.shopnest.com")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()));  // AllowCredentials needed for HttpOnly cookie

var app = builder.Build();

// ── Seed roles on startup ─────────────────────────────────────────────────────
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

// Required for WebApplicationFactory in integration tests
public partial class Program { }
