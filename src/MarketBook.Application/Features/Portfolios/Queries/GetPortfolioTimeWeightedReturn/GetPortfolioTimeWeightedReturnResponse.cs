// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn;

/// <summary>EN: One TWR sub-period. FA: یک زیر‌دوره TWR.</summary>
public sealed record PortfolioTwrSegmentResponse(
    DateTimeOffset Start,
    DateTimeOffset End,
    decimal? BeginningNetAssetValueBase,
    decimal? EndingNetAssetValueBase,
    bool IsComplete,
    bool IsCalculable,
    decimal? Return);

/// <summary>EN: Historical portfolio time-weighted return response. FA: پاسخ بازده زمانی‌وزن تاریخی پرتفوی.</summary>
public sealed record GetPortfolioTimeWeightedReturnResponse(
    string PortfolioId,
    string BaseCurrencyId,
    DateTimeOffset From,
    DateTimeOffset To,
    int ExternalFlowBoundaryCount,
    bool IsComplete,
    bool IsCalculable,
    decimal? TimeWeightedReturn,
    IReadOnlyCollection<PortfolioTwrSegmentResponse> Segments);
