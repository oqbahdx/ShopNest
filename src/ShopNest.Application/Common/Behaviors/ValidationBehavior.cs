using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
	{
		if (!_validators.Any())
		{
			return await next();
		}
		ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);
		List<ValidationFailure> failures = (from f in _validators.Select((IValidator<TRequest> v) => v.Validate(context)).SelectMany((ValidationResult r) => r.Errors)
			where f != null
			select f).ToList();
		if (failures.Count == 0)
		{
			return await next();
		}
		string errorMessage = string.Join(" | ", failures.Select((ValidationFailure f) => f.ErrorMessage));
		Type resultType = typeof(TResponse);
		if (resultType == typeof(Result))
		{
			return (TResponse)(object)Result.Failure(errorMessage, "VALIDATION_ERROR");
		}
		if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
		{
			Type innerType = resultType.GetGenericArguments()[0];
			MethodInfo failureMethod = typeof(Result<>).MakeGenericType(innerType).GetMethod("Failure", new Type[2]
			{
				typeof(string),
				typeof(string)
			})!;
			return (TResponse)failureMethod.Invoke(null, new object[2] { errorMessage, "VALIDATION_ERROR" })!;
		}
		throw new ValidationException(failures);
	}
}
