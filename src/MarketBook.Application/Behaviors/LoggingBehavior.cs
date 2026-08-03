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
/// EN: Logs requests.
/// FA: درخواست‌ها را ثبت می‌کند.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILoggerAdapter<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public LoggingBehavior(
        ILoggerAdapter<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling request {RequestName}",
            typeof(TRequest).Name);

        var response = await next();

        _logger.LogInformation(
            "Request {RequestName} completed",
            typeof(TRequest).Name);

        return response;
    }
}