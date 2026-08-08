// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Infrastructure.Logging
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace MarketBook.Infrastructure.Logging;

/// <summary>
/// EN: Adapts Microsoft logging to the Application logging abstraction.
/// FA: لاگر Microsoft را به abstraction لایه Application متصل می‌کند.
/// </summary>
/// <typeparam name="T">
/// EN: Logging category type.
/// FA: نوع دسته لاگ.
/// </typeparam>
public sealed class LoggerAdapter<T> : ILoggerAdapter<T>
{
    private readonly ILogger<T> _logger;

    /// <summary>
    /// EN: Initializes a new logger adapter.
    /// FA: نمونه جدیدی از آداپتور لاگر را ایجاد می‌کند.
    /// </summary>
    public LoggerAdapter(ILogger<T> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc />
    public void LogInformation(
        string message,
        params object?[] args)
    {
        _logger.LogInformation(message, args);
    }

    /// <inheritdoc />
    public void LogWarning(
        string message,
        params object?[] args)
    {
        _logger.LogWarning(message, args);
    }

    /// <inheritdoc />
    public void LogError(
        Exception exception,
        string message,
        params object?[] args)
    {
        _logger.LogError(exception, message, args);
    }
}
