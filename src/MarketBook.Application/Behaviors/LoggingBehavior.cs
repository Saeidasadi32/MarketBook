// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Application.Behaviors
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Logging;
using MediatR;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Logs application requests.
/// FA: درخواست‌های Application را ثبت می‌کند.
/// </summary>
/// <typeparam name="TRequest">EN: Request type. FA: نوع درخواست.</typeparam>
/// <typeparam name="TResponse">EN: Response type. FA: نوع پاسخ.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILoggerAdapter<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// EN: Initializes a new instance of the logging behavior.
    /// FA: نمونه جدیدی از رفتار ثبت لاگ را ایجاد می‌کند.
    /// </summary>
    /// <param name="logger">EN: Logger adapter. FA: آداپتور لاگر.</param>
    public LoggingBehavior(
        ILoggerAdapter<LoggingBehavior<TRequest, TResponse>> logger)
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

        _logger.LogInformation(
            "Handling request {RequestName}",
            typeof(TRequest).Name);

        TResponse response = await next(cancellationToken);

        _logger.LogInformation(
            "Request {RequestName} completed",
            typeof(TRequest).Name);

        return response;
    }
}
