// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ActivatePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ActivatePortfolioRiskPolicy;

/// <summary>EN: Atomically switches the active policy version. FA: نسخه فعال Policy را اتمیک جابه‌جا می‌کند.</summary>
public sealed class ActivatePortfolioRiskPolicyHandler
    : IRequestHandler<ActivatePortfolioRiskPolicyCommand, Result<PortfolioRiskPolicyResponse>>
{
    private readonly IPortfolioRiskPolicyRepository _policies;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public ActivatePortfolioRiskPolicyHandler(
        IPortfolioRiskPolicyRepository policies,
        IApplicationDbContext dbContext)
    {
        _policies = policies;
        _dbContext = dbContext;
    }

    /// <summary>EN: Archives the old active version and activates the selected version. FA: نسخه فعال قبلی را بایگانی و نسخه انتخابی را فعال می‌کند.</summary>
    public async Task<Result<PortfolioRiskPolicyResponse>> Handle(
        ActivatePortfolioRiskPolicyCommand request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        PortfolioRiskPolicy? selected =
            await _policies.GetByVersionAsync(portfolioId, request.PolicyVersion, cancellationToken);

        if (selected is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.NotFound", "The requested risk-policy version was not found."));
        }

        PortfolioRiskPolicy? active =
            await _policies.GetActiveAsync(portfolioId, cancellationToken);

        if (active is not null && active.Id != selected.Id)
        {
            active.Archive(request.EffectiveFrom);
        }

        selected.Activate(request.EffectiveFrom);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioRiskPolicyResponse>.Success(
            PortfolioRiskPolicyResponse.FromDomain(selected));
    }
}
