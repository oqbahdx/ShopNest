namespace ShopNest.Application.Common.Models;

/// <summary>
/// Discriminated-union result — avoids throwing for expected business failures.
/// </summary>
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
        => (IsSuccess, Data, Error, ErrorCode) = (ok, data, err, code);

    public static Result<T> Success(T data)
        => new(true, data, null, null);

    public static Result<T> Failure(string error, string code = ErrorCodes.IdentityError)
        => new(false, default, error, code);

    public Result<TOut> Map<TOut>(Func<T, TOut> fn) =>
        IsSuccess
            ? Result<TOut>.Success(fn(Data!))
            : Result<TOut>.Failure(Error!, ErrorCode!);
}

/// <summary>Non-generic result for void commands.</summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorMessage => Error;
    public string? ErrorCode { get; }

    private Result(bool ok, string? err, string? code)
        => (IsSuccess, Error, ErrorCode) = (ok, err, code);

    public static Result Success()
        => new(true, null, null);

    public static Result Failure(string error, string code = ErrorCodes.IdentityError)
        => new(false, error, code);
}
