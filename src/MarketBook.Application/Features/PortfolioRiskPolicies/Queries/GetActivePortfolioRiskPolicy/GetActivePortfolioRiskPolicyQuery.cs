// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetActivePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetActivePortfolioRiskPolicy;

/// <summary>EN: Gets the active persisted policy. FA: Policy فعال ذخیره‌شده را دریافت می‌کند.</summary>
public sealed record GetActivePortfolioRiskPolicyQuery(string PortfolioId)
    : IRequest<Result<PortfolioRiskPolicyResponse>>;
