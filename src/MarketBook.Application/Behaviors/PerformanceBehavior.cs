// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Behaviors
// -----------------------------------------------------------------------------

using System.Diagnostics;
using MediatR;
using MarketBook.Application.Abstractions.Logging;

namespace MarketBook.Application.Behaviors;

/// <summary>
/// EN: Measures request execution time.
/// FA: زمان اجرای درخواست را اندازه‌گیری می‌کند.
/// </summary>
public sealed class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILoggerAdapter<PerformanceBehavior<TRequest, TResponse>> _logger;
    
   /// <summary>
   /// 
   /// </summary>
   /// <param name="logger"></param>
    public PerformanceBehavior(
        ILoggerAdapter<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        _logger.LogInformation(
            "{Request} executed in {Elapsed} ms.",
            typeof(TRequest).Name,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}