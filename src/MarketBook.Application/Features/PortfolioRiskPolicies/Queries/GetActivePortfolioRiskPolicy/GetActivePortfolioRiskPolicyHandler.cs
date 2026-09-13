// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetActivePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetActivePortfolioRiskPolicy;

/// <summary>EN: Handles active-policy lookup. FA: دریافت Policy فعال را مدیریت می‌کند.</summary>
public sealed class GetActivePortfolioRiskPolicyHandler
    : IRequestHandler<GetActivePortfolioRiskPolicyQuery, Result<PortfolioRiskPolicyResponse>>
{
    private readonly IPortfolioRiskPolicyRepository _policies;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetActivePortfolioRiskPolicyHandler(IPortfolioRiskPolicyRepository policies)
    {
        _policies = policies;
    }

    /// <summary>EN: Gets the active version. FA: نسخه فعال را دریافت می‌کند.</summary>
    public async Task<Result<PortfolioRiskPolicyResponse>> Handle(
        GetActivePortfolioRiskPolicyQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        PortfolioRiskPolicy? policy = await _policies.GetActiveAsync(portfolioId, cancellationToken);
        return policy is null
            ? Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.NotFound", "No active risk policy was found."))
            : Result<PortfolioRiskPolicyResponse>.Success(
                PortfolioRiskPolicyResponse.FromDomain(policy));
    }
}
