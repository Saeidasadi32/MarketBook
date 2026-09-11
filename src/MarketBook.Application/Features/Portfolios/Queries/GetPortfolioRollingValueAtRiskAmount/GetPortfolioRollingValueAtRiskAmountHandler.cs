// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRiskAmount
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRisk;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioRollingValueAtRiskAmount;

/// <summary>
/// EN: Composes DOC-0038 rolling historical VaR/CVaR with DOC-0024 historical translated NAV at each window end.
/// FA: VaR/CVaR تاریخی Rolling از DOC-0038 را با NAV تاریخی ترجمه‌شده DOC-0024 در پایان هر پنجره ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioRollingValueAtRiskAmountHandler
    : IRequestHandler<GetPortfolioRollingValueAtRiskAmountQuery, Result<GetPortfolioRollingValueAtRiskAmountResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the rolling currency-amount VaR/CVaR handler.
    /// FA: Handler مربوط به VaR/CVaR مبلغی Rolling را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioRollingValueAtRiskAmountHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates rolling VaR/CVaR monetary exposure using historical NAV at each WindowTo.
    /// FA: exposure مبلغی VaR/CVaR Rolling را با NAV تاریخی هر WindowTo محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Rolling amount-risk request. FA: درخواست ریسک مبلغی Rolling.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Rolling historical VaR/CVaR amount analytics. FA: تحلیل مبلغی VaR/CVaR تاریخی Rolling.</returns>
    public async Task<Result<GetPortfolioRollingValueAtRiskAmountResponse>> Handle(
        GetPortfolioRollingValueAtRiskAmountQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioRollingValueAtRiskResponse> rollingResult =
            await _sender.Send(
                new GetPortfolioRollingValueAtRiskQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.WindowPeriods,
                    request.ConfidenceLevel),
                cancellationToken);

        if (rollingResult.IsFailure)
        {
            return Result<GetPortfolioRollingValueAtRiskAmountResponse>.Fail(
                rollingResult.Error);
        }

        GetPortfolioRollingValueAtRiskResponse rolling =
            rollingResult.Value!;

        if (!rolling.IsComplete)
        {
            return Result<GetPortfolioRollingValueAtRiskAmountResponse>.Success(
                new GetPortfolioRollingValueAtRiskAmountResponse(
                    rolling.PortfolioId,
                    rolling.BaseCurrencyId,
                    rolling.From,
                    rolling.To,
                    rolling.Interval,
                    false,
                    rolling.WindowPeriods,
                    rolling.ConfidenceLevel,
                    rolling.TailProbability,
                    0,
                    0,
                    []));
        }

        List<PortfolioRollingValueAtRiskAmountPointResponse> points = [];
        bool allPointsComplete = true;
        int calculablePointCount = 0;

        foreach (PortfolioRollingValueAtRiskPointResponse rollingPoint in rolling.Points)
        {
            Result<GetPortfolioTranslatedNavAsOfResponse> navResult =
                await _sender.Send(
                    new GetPortfolioTranslatedNavAsOfQuery(
                        request.PortfolioId,
                        rollingPoint.WindowTo),
                    cancellationToken);

            if (navResult.IsFailure)
            {
                return Result<GetPortfolioRollingValueAtRiskAmountResponse>.Fail(
                    navResult.Error);
            }

            GetPortfolioTranslatedNavAsOfResponse nav =
                navResult.Value!;

            if (!string.Equals(
                    rolling.BaseCurrencyId,
                    nav.BaseCurrencyId,
                    StringComparison.Ordinal))
            {
                return Result<GetPortfolioRollingValueAtRiskAmountResponse>.Fail(
                    new Error(
                        "Portfolio.RollingValueAtRiskAmount.BaseCurrencyMismatch",
                        "Rolling historical VaR and historical NAV resolved different base currencies."));
            }

            bool pointComplete =
                nav.IsComplete;

            allPointsComplete &=
                pointComplete;

            decimal? netAssetValueBase =
                pointComplete
                    ? nav.NetAssetValueBase
                    : null;

            bool pointCalculable =
                pointComplete &&
                netAssetValueBase.HasValue &&
                netAssetValueBase.Value > 0m;

            decimal? valueAtRiskAmountBase =
                pointCalculable
                    ? netAssetValueBase!.Value * rollingPoint.ValueAtRiskReturn
                    : null;

            decimal? conditionalValueAtRiskAmountBase =
                pointCalculable
                    ? netAssetValueBase!.Value * rollingPoint.ConditionalValueAtRiskReturn
                    : null;

            if (pointCalculable)
            {
                calculablePointCount++;
            }

            points.Add(
                new PortfolioRollingValueAtRiskAmountPointResponse(
                    rollingPoint.WindowFrom,
                    rollingPoint.WindowTo,
                    rollingPoint.ObservationCount,
                    rollingPoint.TailObservationCount,
                    pointComplete,
                    pointCalculable,
                    netAssetValueBase,
                    rollingPoint.HistoricalQuantileReturn,
                    rollingPoint.ValueAtRiskReturn,
                    rollingPoint.ConditionalValueAtRiskReturn,
                    valueAtRiskAmountBase,
                    conditionalValueAtRiskAmountBase));
        }

        return Result<GetPortfolioRollingValueAtRiskAmountResponse>.Success(
            new GetPortfolioRollingValueAtRiskAmountResponse(
                rolling.PortfolioId,
                rolling.BaseCurrencyId,
                rolling.From,
                rolling.To,
                rolling.Interval,
                allPointsComplete,
                rolling.WindowPeriods,
                rolling.ConfidenceLevel,
                rolling.TailProbability,
                points.Count,
                calculablePointCount,
                points));
    }
}
