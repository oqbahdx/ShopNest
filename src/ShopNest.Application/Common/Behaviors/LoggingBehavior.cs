using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ShopNest.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
	{
		_logger = logger;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
	{
		string requestName = typeof(TRequest).Name;
		_logger.LogDebug("Handling {Request}", requestName);
		Stopwatch sw = Stopwatch.StartNew();
		try
		{
			TResponse response = await next();
			sw.Stop();
			if (sw.ElapsedMilliseconds > 500)
			{
				_logger.LogWarning("Slow handler detected: {Request} took {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
			}
			else
			{
				_logger.LogDebug("{Request} handled in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
			}
			return response;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			sw.Stop();
			_logger.LogError(ex2, "{Request} failed after {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
			throw;
		}
	}
}
