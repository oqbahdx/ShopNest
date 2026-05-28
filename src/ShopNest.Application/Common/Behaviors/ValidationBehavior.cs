using FluentValidation;
using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs all FluentValidation validators before
/// any handler executes. Returns a validation failure Result instead of throwing.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        // Build a clean validation error message
        var errorMessage = string.Join(" | ", failures.Select(f => f.ErrorMessage));

        // Attempt to return a typed Result failure without throwing
        var resultType = typeof(TResponse);

        if (resultType == typeof(Result))
            return (TResponse)(object)Result.Failure(errorMessage, ErrorCodes.ValidationError);

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType   = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(Result<object>.Failure),
                    new[] { typeof(string), typeof(string) })!;

            return (TResponse)failureMethod.Invoke(null,
                new object[] { errorMessage, ErrorCodes.ValidationError })!;
        }

        // Fallback: throw for non-Result return types
        throw new ValidationException(failures);
    }
}
