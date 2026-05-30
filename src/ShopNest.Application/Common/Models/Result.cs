using System;

namespace ShopNest.Application.Common.Models;

public sealed class Result<T>
{
	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public T? Data { get; }

	public T? Value => Data;

	public string? Error { get; }

	public string? ErrorMessage => Error;

	public string? ErrorCode { get; }

	private Result(bool ok, T? data, string? err, string? code)
	{
		bool flag = ok;
		IsSuccess = flag;
		Data = data;
		Error = err;
		ErrorCode = code;
	}

	public static Result<T> Success(T data)
	{
		return new Result<T>(ok: true, data, null, null);
	}

	public static Result<T> Failure(string error, string code = "AUTH_IDENTITY_ERROR")
	{
		return new Result<T>(ok: false, default(T), error, code);
	}

	public Result<TOut> Map<TOut>(Func<T, TOut> fn)
	{
		return IsSuccess
			? Result<TOut>.Success(fn(Data!))
			: Result<TOut>.Failure(Error ?? string.Empty, ErrorCode ?? "AUTH_IDENTITY_ERROR");
	}
}
public sealed class Result
{
	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public string? Error { get; }

	public string? ErrorMessage => Error;

	public string? ErrorCode { get; }

	private Result(bool ok, string? err, string? code)
	{
		bool flag = ok;
		IsSuccess = flag;
		Error = err;
		ErrorCode = code;
	}

	public static Result Success()
	{
		return new Result(ok: true, null, null);
	}

	public static Result Failure(string error, string code = "AUTH_IDENTITY_ERROR")
	{
		return new Result(ok: false, error, code);
	}
}
