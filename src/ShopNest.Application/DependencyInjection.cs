using System;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ShopNest.Application.Common.Behaviors;

namespace ShopNest.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddMediatR(delegate(MediatRServiceConfiguration cfg)
		{
			cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
			cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
		});
		services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
		services.AddAutoMapper((Action<IMapperConfigurationExpression>)delegate
		{
		}, new Assembly[1] { typeof(DependencyInjection).Assembly });
		return services;
	}
}
