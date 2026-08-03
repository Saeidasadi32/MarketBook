// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Behaviors
// -----------------------------------------------------------------------------

using MediatR;
using MarketBook.Application.Abstractions.Logging;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Logs unhandled exceptions.
/// FA: استثناهای مدیریت‌نشده را ثبت می‌کند.
/// </summary>
public sealed class UnhandledExceptionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILoggerAdapter<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public UnhandledExceptionBehavior(
        ILoggerAdapter<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception while executing {RequestName}",
                typeof(TRequest).Name);

            throw;
        }
    }
}