// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.Common
// -----------------------------------------------------------------------------

namespace MarketBook.Domain.Common;

/// <summary>
/// EN: Represents an operation result.
/// FA: نتیجه اجرای یک عملیات.
/// </summary>
public class Result
{
    protected Result(
        bool isSuccess,
        Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// EN: Gets success flag.
    /// FA: وضعیت موفقیت.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// EN: Gets failure flag.
    /// FA: وضعیت شکست.
    /// </summary>
    public bool IsFailure
        => !IsSuccess;

    /// <summary>
    /// EN: Gets operation error.
    /// FA: خطای عملیات.
    /// </summary>
    public Error Error { get; }

    public static Result Success()
        => new(true, Error.None);

    public static Result Failure(Error error)
        => new(false, error);
}

/// <summary>
/// EN: Represents an operation result with value.
/// FA: نتیجه عملیات همراه با مقدار.
/// </summary>
public sealed class Result<T> : Result
{
    private Result(
        T? value,
        bool isSuccess,
        Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// EN: Gets returned value.
    /// FA: مقدار بازگشتی.
    /// </summary>
    public T? Value { get; }

    public static Result<T> Success(T value)
        => new(value, true, Error.None);

    public static Result<T> Failure(Error error)
        => new(default, false, error);
}