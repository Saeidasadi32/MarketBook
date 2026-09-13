// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetPortfolioRiskPolicyHistory
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetPortfolioRiskPolicyHistory;

/// <summary>EN: Gets persisted policy history. FA: تاریخچه Policy ذخیره‌شده را دریافت می‌کند.</summary>
public sealed record GetPortfolioRiskPolicyHistoryQuery(string PortfolioId)
    : IRequest<Result<IReadOnlyCollection<PortfolioRiskPolicyResponse>>>;
