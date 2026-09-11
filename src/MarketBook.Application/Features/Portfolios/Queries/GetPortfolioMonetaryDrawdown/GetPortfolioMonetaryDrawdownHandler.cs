// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioDrawdown;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMonetaryDrawdown;

/// <summary>
/// EN: Composes DOC-0031 cash-flow-neutral drawdown with DOC-0024 historical translated NAV.
/// FA: Drawdown خنثی نسبت به جریان سرمایه از DOC-0031 را با NAV تاریخی ترجمه‌شده DOC-0024 ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioMonetaryDrawdownHandler
    : IRequestHandler<GetPortfolioMonetaryDrawdownQuery, Result<GetPortfolioMonetaryDrawdownResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the monetary drawdown handler.
    /// FA: Handler افت سرمایه مبلغی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioMonetaryDrawdownHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Translates relative drawdown into a base-currency loss amount at each historical exposure point.
    /// FA: افت نسبی را در هر نقطه exposure تاریخی به مبلغ زیان در ارز پایه تبدیل می‌کند.
    /// </summary>
    /// <param name="request">EN: Monetary drawdown request. FA: درخواست افت سرمایه مبلغی.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Historical monetary drawdown analytics. FA: تحلیل تاریخی افت سرمایه مبلغی.</returns>
    public async Task<Result<GetPortfolioMonetaryDrawdownResponse>> Handle(
        GetPortfolioMonetaryDrawdownQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioDrawdownResponse> drawdownResult =
            await _sender.Send(
                new GetPortfolioDrawdownQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval),
                cancellationToken);

        if (drawdownResult.IsFailure)
        {
            return Result<GetPortfolioMonetaryDrawdownResponse>.Fail(
                drawdownResult.Error);
        }

        GetPortfolioDrawdownResponse drawdown =
            drawdownResult.Value!;

        if (!drawdown.IsComplete)
        {
            return Result<GetPortfolioMonetaryDrawdownResponse>.Success(
                new GetPortfolioMonetaryDrawdownResponse(
                    drawdown.PortfolioId,
                    drawdown.BaseCurrencyId,
                    drawdown.From,
                    drawdown.To,
                    drawdown.Interval,
                    false,
                    drawdown.CurrentDrawdown,
                    null,
                    drawdown.MaximumDrawdown,
                    null,
                    drawdown.MaximumDrawdownPeakTimestamp,
                    drawdown.MaximumDrawdownTroughTimestamp,
                    []));
        }

        List<PortfolioMonetaryDrawdownPointResponse> points = [];
        bool allPointsComplete = true;

        foreach (PortfolioDrawdownPointResponse drawdownPoint in drawdown.Points)
        {
            Result<GetPortfolioTranslatedNavAsOfResponse> navResult =
                await _sender.Send(
                    new GetPortfolioTranslatedNavAsOfQuery(
                        request.PortfolioId,
                        drawdownPoint.Timestamp),
                    cancellationToken);

            if (navResult.IsFailure)
            {
                return Result<GetPortfolioMonetaryDrawdownResponse>.Fail(
                    navResult.Error);
            }

            GetPortfolioTranslatedNavAsOfResponse nav =
                navResult.Value!;

            if (!string.Equals(
                    drawdown.BaseCurrencyId,
                    nav.BaseCurrencyId,
                    StringComparison.Ordinal))
            {
                return Result<GetPortfolioMonetaryDrawdownResponse>.Fail(
                    new Error(
                        "Portfolio.MonetaryDrawdown.BaseCurrencyMismatch",
                        "Drawdown and historical NAV resolved different base currencies."));
            }

            bool pointComplete =
                drawdownPoint.IsCalculable &&
                drawdownPoint.Drawdown.HasValue &&
                nav.IsComplete &&
                nav.NetAssetValueBase.HasValue;

            allPointsComplete &=
                pointComplete;

            decimal? equivalentPeakNetAssetValueBase = null;
            decimal? drawdownAmountBase = null;
            bool pointCalculable = false;

            if (pointComplete)
            {
                decimal navBase =
                    nav.NetAssetValueBase!.Value;

                decimal drawdownValue =
                    drawdownPoint.Drawdown!.Value;

                decimal remainingRatio =
                    1m + drawdownValue;

                if (navBase >= 0m &&
                    remainingRatio > 0m)
                {
                    equivalentPeakNetAssetValueBase =
                        navBase / remainingRatio;

                    drawdownAmountBase =
                        equivalentPeakNetAssetValueBase.Value - navBase;

                    pointCalculable = true;
                }
            }

            points.Add(
                new PortfolioMonetaryDrawdownPointResponse(
                    drawdownPoint.Timestamp,
                    pointComplete,
                    pointCalculable,
                    nav.IsComplete
                        ? nav.NetAssetValueBase
                        : null,
                    drawdownPoint.WealthIndex,
                    drawdownPoint.RunningPeakWealthIndex,
                    drawdownPoint.RunningPeakTimestamp,
                    drawdownPoint.Drawdown,
                    equivalentPeakNetAssetValueBase,
                    drawdownAmountBase));
        }

        decimal? currentDrawdownAmountBase =
            points.Count > 0
                ? points[^1].DrawdownAmountBase
                : null;

        decimal? maximumDrawdownAmountBase = null;

        if (drawdown.MaximumDrawdownTroughTimestamp.HasValue)
        {
            PortfolioMonetaryDrawdownPointResponse? maximumPoint =
                points.FirstOrDefault(
                    item =>
                        item.Timestamp ==
                        drawdown.MaximumDrawdownTroughTimestamp.Value);

            maximumDrawdownAmountBase =
                maximumPoint?.DrawdownAmountBase;
        }

        return Result<GetPortfolioMonetaryDrawdownResponse>.Success(
            new GetPortfolioMonetaryDrawdownResponse(
                drawdown.PortfolioId,
                drawdown.BaseCurrencyId,
                drawdown.From,
                drawdown.To,
                drawdown.Interval,
                allPointsComplete,
                drawdown.CurrentDrawdown,
                currentDrawdownAmountBase,
                drawdown.MaximumDrawdown,
                maximumDrawdownAmountBase,
                drawdown.MaximumDrawdownPeakTimestamp,
                drawdown.MaximumDrawdownTroughTimestamp,
                points));
    }
}
