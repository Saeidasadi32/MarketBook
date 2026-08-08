// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Namespace : MarketBook.Application.Behaviors
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Logging;
using MediatR;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Logs unhandled application exceptions.
/// FA: استثناهای مدیریت‌نشده Application را ثبت می‌کند.
/// </summary>
/// <typeparam name="TRequest">EN: Request type. FA: نوع درخواست.</typeparam>
/// <typeparam name="TResponse">EN: Response type. FA: نوع پاسخ.</typeparam>
public sealed class UnhandledExceptionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILoggerAdapter<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// EN: Initializes a new instance of the unhandled exception behavior.
    /// FA: نمونه جدیدی از رفتار مدیریت استثنا را ایجاد می‌کند.
    /// </summary>
    /// <param name="logger">EN: Logger adapter. FA: آداپتور لاگر.</param>
    public UnhandledExceptionBehavior(
        ILoggerAdapter<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
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

        try
        {
            return await next(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while executing {RequestName}",
                typeof(TRequest).Name);

            throw;
        }
    }
}
