// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRisk;
using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioValueAtRiskAmount;

/// <summary>
/// EN: Composes DOC-0037 historical VaR/CVaR with DOC-0024 historical translated NAV.
/// FA: VaR/CVaR تاریخی DOC-0037 را با NAV تاریخی ترجمه‌شده DOC-0024 ترکیب می‌کند.
/// </summary>
public sealed class GetPortfolioValueAtRiskAmountHandler
    : IRequestHandler<GetPortfolioValueAtRiskAmountQuery, Result<GetPortfolioValueAtRiskAmountResponse>>
{
    private readonly ISender _sender;

    /// <summary>
    /// EN: Initializes the currency-amount VaR/CVaR handler.
    /// FA: Handler مربوط به VaR/CVaR مبلغی را مقداردهی می‌کند.
    /// </summary>
    /// <param name="sender">EN: MediatR sender. FA: Sender مدیاتور.</param>
    public GetPortfolioValueAtRiskAmountHandler(ISender sender)
    {
        ArgumentNullException.ThrowIfNull(sender);
        _sender = sender;
    }

    /// <summary>
    /// EN: Calculates VaR/CVaR amounts from historical ratios and the complete NAV at To.
    /// FA: مبالغ VaR/CVaR را از نسبت‌های تاریخی و NAV کامل در لحظه To محاسبه می‌کند.
    /// </summary>
    /// <param name="request">EN: Currency-amount VaR/CVaR request. FA: درخواست VaR/CVaR مبلغی.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>EN: Historical VaR/CVaR amount analytics. FA: تحلیل مبلغی VaR/CVaR تاریخی.</returns>
    public async Task<Result<GetPortfolioValueAtRiskAmountResponse>> Handle(
        GetPortfolioValueAtRiskAmountQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GetPortfolioValueAtRiskResponse> riskResult =
            await _sender.Send(
                new GetPortfolioValueAtRiskQuery(
                    request.PortfolioId,
                    request.From,
                    request.To,
                    request.Interval,
                    request.ConfidenceLevel),
                cancellationToken);

        if (riskResult.IsFailure)
        {
            return Result<GetPortfolioValueAtRiskAmountResponse>.Fail(
                riskResult.Error);
        }

        Result<GetPortfolioTranslatedNavAsOfResponse> navResult =
            await _sender.Send(
                new GetPortfolioTranslatedNavAsOfQuery(
                    request.PortfolioId,
                    request.To),
                cancellationToken);

        if (navResult.IsFailure)
        {
            return Result<GetPortfolioValueAtRiskAmountResponse>.Fail(
                navResult.Error);
        }

        GetPortfolioValueAtRiskResponse risk =
            riskResult.Value!;

        GetPortfolioTranslatedNavAsOfResponse nav =
            navResult.Value!;

        if (!string.Equals(
                risk.BaseCurrencyId,
                nav.BaseCurrencyId,
                StringComparison.Ordinal))
        {
            return Result<GetPortfolioValueAtRiskAmountResponse>.Fail(
                new Error(
                    "Portfolio.ValueAtRiskAmount.BaseCurrencyMismatch",
                    "Historical VaR and historical NAV resolved different base currencies."));
        }

        bool isComplete =
            risk.IsComplete &&
            nav.IsComplete;

        decimal? netAssetValueBase =
            nav.IsComplete
                ? nav.NetAssetValueBase
                : null;

        bool hasPositiveNav =
            netAssetValueBase.HasValue &&
            netAssetValueBase.Value > 0m;

        bool isCalculable =
            isComplete &&
            risk.IsCalculable &&
            risk.ValueAtRiskReturn.HasValue &&
            risk.ConditionalValueAtRiskReturn.HasValue &&
            hasPositiveNav;

        decimal? valueAtRiskAmountBase =
            isCalculable
                ? netAssetValueBase!.Value * risk.ValueAtRiskReturn!.Value
                : null;

        decimal? conditionalValueAtRiskAmountBase =
            isCalculable
                ? netAssetValueBase!.Value * risk.ConditionalValueAtRiskReturn!.Value
                : null;

        return Result<GetPortfolioValueAtRiskAmountResponse>.Success(
            new GetPortfolioValueAtRiskAmountResponse(
                risk.PortfolioId,
                risk.BaseCurrencyId,
                risk.From,
                risk.To,
                risk.Interval,
                risk.ConfidenceLevel,
                isComplete,
                isCalculable,
                risk.ObservationCount,
                risk.TailObservationCount,
                netAssetValueBase,
                risk.ValueAtRiskReturn,
                risk.ConditionalValueAtRiskReturn,
                valueAtRiskAmountBase,
                conditionalValueAtRiskAmountBase));
    }
}
