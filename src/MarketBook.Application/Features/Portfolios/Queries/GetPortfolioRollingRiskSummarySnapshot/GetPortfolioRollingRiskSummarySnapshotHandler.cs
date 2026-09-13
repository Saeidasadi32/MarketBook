// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRiskSummarySnapshot
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRisk;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRiskAmount;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingRiskSummarySnapshot;

/// <summary>
/// EN: Composes the latest DOC-0036 and DOC-0040 rolling points into a compact dashboard snapshot.
/// FA: آخرین نقاط Rolling از DOC-0036 و DOC-0040 را در یک snapshot فشرده داشبورد ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioRollingRiskSummarySnapshotHandler
    : IRequestHandler<GetPortfolioRollingRiskSummarySnapshotQuery, Result<GetPortfolioRollingRiskSummarySnapshotResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the rolling risk-summary snapshot handler.
    /// FA: Handler مربوط به snapshot خلاصه ریسک Rolling را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRollingRiskSummarySnapshotHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Returns only the latest available full-window point from each rolling source.
    /// FA: فقط آخرین نقطه موجود با پنجره کامل را از هر منبع Rolling برمی‌گرداند.
    /// </summary>
    /// <param name="request">EN: Rolling snapshot request. FA: درخواست snapshot Rolling.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Compact latest rolling-risk snapshot. FA: snapshot فشرده آخرین ریسک Rolling.</returns>
    public async Task<Result<GetPortfolioRollingRiskSummarySnapshotResponse>> Handle(
        GetPortfolioRollingRiskSummarySnapshotQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioRollingRiskResponse> rollingRiskResult =
            await _sender.Send(
                new GetPortfolioRollingRiskQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.WindowPeriods,
                    request.RiskFreeRateAnnual,
                    request.MinimumAcceptableReturnAnnual),
                cancellationToken);

        if (rollingRiskResult.IsFailure)
        {
            return Result<GetPortfolioRollingRiskSummarySnapshotResponse>.Fail(
                rollingRiskResult.Error);
        }

        Result<GetPortfolioRollingValueAtRiskAmountResponse> rollingValueAtRiskResult =
            await _sender.Send(
                new GetPortfolioRollingValueAtRiskAmountQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.WindowPeriods,
                    request.ConfidenceLevel),
                cancellationToken);

        if (rollingValueAtRiskResult.IsFailure)
        {
            return Result<GetPortfolioRollingRiskSummarySnapshotResponse>.Fail(
                rollingValueAtRiskResult.Error);
        }

        GetPortfolioRollingRiskResponse rollingRisk =
            rollingRiskResult.Value!;

        GetPortfolioRollingValueAtRiskAmountResponse rollingValueAtRisk =
            rollingValueAtRiskResult.Value!;

        if (!string.Equals(
                rollingRisk.BaseCurrencyId,
                rollingValueAtRisk.BaseCurrencyId,
                StringComparison.Ordinal))
        {
            return Result<GetPortfolioRollingRiskSummarySnapshotResponse>.Fail(
                new Error(
                    "Portfolio.RollingRiskSummary.BaseCurrencyMismatch",
                    "Rolling risk and rolling monetary VaR resolved different base currencies."));
        }

        PortfolioRollingRiskPointResponse? latestRiskPoint =
            rollingRisk.Points.LastOrDefault();

        PortfolioRollingValueAtRiskAmountPointResponse? latestValueAtRiskPoint =
            rollingValueAtRisk.Points.LastOrDefault();

        PortfolioRollingRiskLatestSnapshotResponse riskSnapshot =
            new(
                rollingRisk.IsComplete,
                latestRiskPoint is not null,
                rollingRisk.PointCount,
                latestRiskPoint?.WindowFrom,
                latestRiskPoint?.WindowTo,
                latestRiskPoint?.ObservationCount,
                latestRiskPoint?.MeanPeriodicReturn,
                latestRiskPoint?.AnnualizedVolatility,
                latestRiskPoint?.AnnualizedDownsideDeviation,
                latestRiskPoint?.SharpeRatio,
                latestRiskPoint?.SortinoRatio);

        PortfolioRollingValueAtRiskLatestSnapshotResponse valueAtRiskSnapshot =
            new(
                rollingValueAtRisk.IsComplete,
                latestValueAtRiskPoint is not null,
                rollingValueAtRisk.PointCount,
                rollingValueAtRisk.CalculablePointCount,
                latestValueAtRiskPoint?.WindowFrom,
                latestValueAtRiskPoint?.WindowTo,
                latestValueAtRiskPoint?.ObservationCount,
                latestValueAtRiskPoint?.TailObservationCount,
                latestValueAtRiskPoint?.IsCalculable ?? false,
                latestValueAtRiskPoint?.NetAssetValueBase,
                latestValueAtRiskPoint?.ValueAtRiskReturn,
                latestValueAtRiskPoint?.ConditionalValueAtRiskReturn,
                latestValueAtRiskPoint?.ValueAtRiskAmountBase,
                latestValueAtRiskPoint?.ConditionalValueAtRiskAmountBase);

        bool isComplete =
            rollingRisk.IsComplete &&
            rollingValueAtRisk.IsComplete;

        return Result<GetPortfolioRollingRiskSummarySnapshotResponse>.Success(
            new GetPortfolioRollingRiskSummarySnapshotResponse(
                rollingRisk.PortfolioId,
                rollingRisk.BaseCurrencyId,
                rollingRisk.From,
                rollingRisk.To,
                rollingRisk.Interval,
                request.WindowPeriods,
                request.ConfidenceLevel,
                request.RiskFreeRateAnnual,
                request.MinimumAcceptableReturnAnnual,
                isComplete,
                riskSnapshot,
                valueAtRiskSnapshot));
    }
}
