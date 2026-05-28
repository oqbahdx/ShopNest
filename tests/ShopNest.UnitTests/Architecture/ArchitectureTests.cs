using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Entities;
using ShopNest.Infrastructure.Persistence;

namespace ShopNest.UnitTests.Architecture;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_layer_does_not_reference_application_infrastructure_or_api()
    {
        var forbidden = new[] { "ShopNest.Application", "ShopNest.Infrastructure", "ShopNest.API" };
        var references = typeof(Product).Assembly.GetReferencedAssemblies().Select(a => a.Name);
        references.Should().NotIntersectWith(forbidden);
    }

    [Fact]
    public void Application_layer_does_not_reference_infrastructure_or_api()
    {
        var forbidden = new[] { "ShopNest.Infrastructure", "ShopNest.API" };
        var references = typeof(ErrorCodes).Assembly.GetReferencedAssemblies().Select(a => a.Name);
        references.Should().NotIntersectWith(forbidden);
    }

    [Fact]
    public void Infrastructure_layer_does_not_reference_api()
    {
        var references = typeof(AppDbContext).Assembly.GetReferencedAssemblies().Select(a => a.Name);
        references.Should().NotContain("ShopNest.API");
    }

    [Fact]
    public void MediatR_requests_are_named_as_commands_or_queries()
    {
        var requestTypes = typeof(ErrorCodes).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(IsMediatRRequest))
            .Where(t => !t.Name.EndsWith("Command", StringComparison.Ordinal) &&
                        !t.Name.EndsWith("Query", StringComparison.Ordinal) &&
                        !t.Name.EndsWith("AuthTokenPair", StringComparison.Ordinal))
            .ToList();

        requestTypes.Should().BeEmpty();
    }

    [Fact]
    public void Api_controllers_have_api_controller_attribute()
    {
        var controllerTypes = typeof(Program).Assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        controllerTypes.Should().NotBeEmpty();
        controllerTypes.All(t => t.GetCustomAttribute<ApiControllerAttribute>() != null).Should().BeTrue();
    }

    private static bool IsMediatRRequest(Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequest<>);
}
