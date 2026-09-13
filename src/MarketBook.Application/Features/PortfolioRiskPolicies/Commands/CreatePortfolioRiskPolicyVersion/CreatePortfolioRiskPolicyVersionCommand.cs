// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicyVersion
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicyVersion;

/// <summary>EN: Creates the next draft policy version. FA: نسخه Draft بعدی Policy را ایجاد می‌کند.</summary>
public sealed record CreatePortfolioRiskPolicyVersionCommand(
    string PortfolioId,
    PortfolioRiskPolicyLimitsRequest Limits)
    : IRequest<Result<PortfolioRiskPolicyResponse>>;
