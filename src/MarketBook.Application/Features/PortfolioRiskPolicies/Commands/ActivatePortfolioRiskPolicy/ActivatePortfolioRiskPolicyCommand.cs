// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ActivatePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ActivatePortfolioRiskPolicy;

/// <summary>EN: Activates one policy version. FA: یک نسخه Policy را فعال می‌کند.</summary>
public sealed record ActivatePortfolioRiskPolicyCommand(
    string PortfolioId,
    int PolicyVersion,
    DateTimeOffset EffectiveFrom)
    : IRequest<Result<PortfolioRiskPolicyResponse>>;
