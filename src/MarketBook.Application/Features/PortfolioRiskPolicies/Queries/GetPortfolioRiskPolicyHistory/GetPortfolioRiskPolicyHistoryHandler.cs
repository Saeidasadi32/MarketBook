// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetPortfolioRiskPolicyHistory
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Queries.GetPortfolioRiskPolicyHistory;

/// <summary>EN: Handles policy-history lookup. FA: دریافت تاریخچه Policy را مدیریت می‌کند.</summary>
public sealed class GetPortfolioRiskPolicyHistoryHandler
    : IRequestHandler<GetPortfolioRiskPolicyHistoryQuery, Result<IReadOnlyCollection<PortfolioRiskPolicyResponse>>>
{
    private readonly IPortfolioRiskPolicyRepository _policies;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetPortfolioRiskPolicyHistoryHandler(IPortfolioRiskPolicyRepository policies)
    {
        _policies = policies;
    }

    /// <summary>EN: Gets versions in ascending business-version order. FA: نسخه‌ها را صعودی دریافت می‌کند.</summary>
    public async Task<Result<IReadOnlyCollection<PortfolioRiskPolicyResponse>>> Handle(
        GetPortfolioRiskPolicyHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<IReadOnlyCollection<PortfolioRiskPolicyResponse>>.Fail(
                new Error("PortfolioRiskPolicy.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        IReadOnlyCollection<PortfolioRiskPolicy> policies =
            await _policies.GetHistoryAsync(portfolioId, cancellationToken);

        IReadOnlyCollection<PortfolioRiskPolicyResponse> response =
            policies.Select(PortfolioRiskPolicyResponse.FromDomain).ToArray();

        return Result<IReadOnlyCollection<PortfolioRiskPolicyResponse>>.Success(response);
    }
}
