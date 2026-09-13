// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ArchivePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ArchivePortfolioRiskPolicy;

/// <summary>EN: Archives one policy version. FA: یک نسخه Policy را بایگانی می‌کند.</summary>
public sealed record ArchivePortfolioRiskPolicyCommand(
    string PortfolioId,
    int PolicyVersion,
    DateTimeOffset EffectiveTo)
    : IRequest<Result<PortfolioRiskPolicyResponse>>;
