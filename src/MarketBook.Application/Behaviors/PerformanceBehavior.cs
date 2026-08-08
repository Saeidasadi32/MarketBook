// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Application.Behaviors
// -----------------------------------------------------------------------------

using System.Diagnostics;
using MarketBook.Application.Abstractions.Logging;
using MediatR;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Measures application request execution time.
/// FA: زمان اجرای درخواست‌های Application را اندازه‌گیری می‌کند.
/// </summary>
/// <typeparam name="TRequest">EN: Request type. FA: نوع درخواست.</typeparam>
/// <typeparam name="TResponse">EN: Response type. FA: نوع پاسخ.</typeparam>
public sealed class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILoggerAdapter<PerformanceBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// EN: Initializes a new instance of the performance behavior.
    /// FA: نمونه جدیدی از رفتار اندازه‌گیری عملکرد را ایجاد می‌کند.
    /// </summary>
    /// <param name="logger">EN: Logger adapter. FA: آداپتور لاگر.</param>
    public PerformanceBehavior(
        ILoggerAdapter<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        Stopwatch stopwatch = Stopwatch.StartNew();

        TResponse response = await next(cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation(
            "{Request} executed in {Elapsed} ms.",
            typeof(TRequest).Name,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
