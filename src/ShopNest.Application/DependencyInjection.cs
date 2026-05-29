using System;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ShopNest.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddMediatR(delegate(MediatRServiceConfiguration cfg)
		{
			cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
		});
		services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
		services.AddAutoMapper((Action<IMapperConfigurationExpression>)delegate
		{
		}, new Assembly[1] { typeof(DependencyInjection).Assembly });
		return services;
	}
}
