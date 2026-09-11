// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioMoneyWeightedReturn;

/// <summary>
/// EN: Requests historical money-weighted return (XIRR) for a portfolio.
/// FA: بازده پول‌وزن تاریخی (XIRR) یک پرتفوی را درخواست می‌کند.
/// </summary>
/// <param name="PortfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
/// <param name="From">EN: Beginning valuation instant. FA: لحظه ارزش‌گذاری ابتدای دوره.</param>
/// <param name="To">EN: Ending valuation instant. FA: لحظه ارزش‌گذاری انتهای دوره.</param>
public sealed record GetPortfolioMoneyWeightedReturnQuery(
    string PortfolioId,
    DateTimeOffset From,
    DateTimeOffset To)
    : IRequest<Result<GetPortfolioMoneyWeightedReturnResponse>>;
