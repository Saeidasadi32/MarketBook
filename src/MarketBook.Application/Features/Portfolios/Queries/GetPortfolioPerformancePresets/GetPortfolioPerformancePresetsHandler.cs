// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformanceComparison;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioPerformancePresets;

/// <summary>
/// EN: Resolves standard dashboard periods and composes the existing TWR/XIRR comparison query for each period.
/// FA: بازه‌های استاندارد داشبورد را resolve کرده و Query مقایسه TWR/XIRR موجود را برای هر بازه ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioPerformancePresetsHandler
    : IRequestHandler<GetPortfolioPerformancePresetsQuery, Result<GetPortfolioPerformancePresetsResponse>>
{
    private readonly ISender _sender;
    private readonly IPortfolioRepository _portfolioRepository;

    /// <summary>
    /// EN: Initializes the performance-preset handler.
    /// FA: Handler بازه‌های استاندارد عملکرد را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender used to compose existing performance comparisons. FA: Sender مدیاتور برای ترکیب مقایسه‌های عملکرد موجود.</param>
    /// <param name="portfolioRepository">EN: Portfolio repository used to resolve inception. FA: Repository پرتفوی برای تعیین زمان inception.</param>
    public GetPortfolioPerformancePresetsHandler(
        ISender sender,
        IPortfolioRepository portfolioRepository)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(portfolioRepository);

        _sender = sender;
        _portfolioRepository = portfolioRepository;
    }

    /// <summary>
    /// EN: Resolves 1M, 3M, 6M, YTD, 1Y, and SinceInception periods using the supplied as-of instant.
    /// FA: بازه‌های 1M، 3M، 6M، YTD، 1Y و SinceInception را با as-of داده‌شده resolve می‌کند.
    /// </summary>
    /// <param name="request">EN: Preset request. FA: درخواست بازه‌های استاندارد.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Standard performance-period projection. FA: Projection دوره‌های استاندارد عملکرد.</returns>
    public async Task<Result<GetPortfolioPerformancePresetsResponse>> Handle(
        GetPortfolioPerformancePresetsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) ||
            portfolioId is null)
        {
            return Result<GetPortfolioPerformancePresetsResponse>.Fail(
                new Error(
                    "PortfolioPerformancePresets.InvalidPortfolioId",
                    "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio =
            await _portfolioRepository.GetByIdAsync(
                portfolioId,
                cancellationToken);

        if (portfolio is null)
        {
            return Result<GetPortfolioPerformancePresetsResponse>.Fail(
                new Error(
                    "PortfolioPerformancePresets.NotFound",
                    "The portfolio was not found."));
        }

        if (request.AsOf <= portfolio.CreatedOn)
        {
            return Result<GetPortfolioPerformancePresetsResponse>.Fail(
                new Error(
                    "PortfolioPerformancePresets.InvalidAsOf",
                    "The as-of instant must be later than the portfolio creation instant."));
        }

        DateTimeOffset yearToDateStart =
            new(
                request.AsOf.Year,
                1,
                1,
                0,
                0,
                0,
                request.AsOf.Offset);

        IReadOnlyCollection<PresetBoundary> boundaries =
        [
            new("1M", request.AsOf.AddMonths(-1)),
            new("3M", request.AsOf.AddMonths(-3)),
            new("6M", request.AsOf.AddMonths(-6)),
            new("YTD", yearToDateStart),
            new("1Y", request.AsOf.AddYears(-1)),
            new("SinceInception", portfolio.CreatedOn)
        ];

        List<PortfolioPerformancePresetResponse> periods = [];
        string? baseCurrencyId = null;

        foreach (PresetBoundary boundary in boundaries)
        {
            Result<GetPortfolioPerformanceComparisonResponse> comparisonResult =
                await _sender.Send(
                    new GetPortfolioPerformanceComparisonQuery(
                        request.PortfolioId,
                        boundary.From,
                        request.AsOf),
                    cancellationToken);

            if (comparisonResult.IsFailure)
            {
                return Result<GetPortfolioPerformancePresetsResponse>.Fail(
                    comparisonResult.Error);
            }

            GetPortfolioPerformanceComparisonResponse comparison =
                comparisonResult.Value!;

            if (baseCurrencyId is null)
            {
                baseCurrencyId = comparison.BaseCurrencyId;
            }
            else if (!string.Equals(
                         baseCurrencyId,
                         comparison.BaseCurrencyId,
                         StringComparison.Ordinal))
            {
                return Result<GetPortfolioPerformancePresetsResponse>.Fail(
                    new Error(
                        "PortfolioPerformancePresets.BaseCurrencyMismatch",
                        "Preset comparisons resolved different base currencies."));
            }

            periods.Add(
                new PortfolioPerformancePresetResponse(
                    boundary.Preset,
                    boundary.From,
                    request.AsOf,
                    comparison.IsDataComplete,
                    comparison.AreBothReturnsAvailable,
                    comparison.TimeWeightedReturn,
                    comparison.MoneyWeightedReturn,
                    comparison.MoneyWeightedMinusTimeWeighted));
        }

        return Result<GetPortfolioPerformancePresetsResponse>.Success(
            new GetPortfolioPerformancePresetsResponse(
                request.PortfolioId,
                baseCurrencyId!,
                request.AsOf,
                periods));
    }

    private sealed record PresetBoundary(
        string Preset,
        DateTimeOffset From);
}
