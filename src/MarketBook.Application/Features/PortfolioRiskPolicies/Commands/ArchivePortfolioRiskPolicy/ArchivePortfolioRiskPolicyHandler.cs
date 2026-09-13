// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ArchivePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.ArchivePortfolioRiskPolicy;

/// <summary>EN: Handles policy archival. FA: بایگانی Policy را مدیریت می‌کند.</summary>
public sealed class ArchivePortfolioRiskPolicyHandler
    : IRequestHandler<ArchivePortfolioRiskPolicyCommand, Result<PortfolioRiskPolicyResponse>>
{
    private readonly IPortfolioRiskPolicyRepository _policies;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public ArchivePortfolioRiskPolicyHandler(
        IPortfolioRiskPolicyRepository policies,
        IApplicationDbContext dbContext)
    {
        _policies = policies;
        _dbContext = dbContext;
    }

    /// <summary>EN: Archives the selected version. FA: نسخه انتخابی را بایگانی می‌کند.</summary>
    public async Task<Result<PortfolioRiskPolicyResponse>> Handle(
        ArchivePortfolioRiskPolicyCommand request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        PortfolioRiskPolicy? policy =
            await _policies.GetByVersionAsync(portfolioId, request.PolicyVersion, cancellationToken);

        if (policy is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.NotFound", "The requested risk-policy version was not found."));
        }

        policy.Archive(request.EffectiveTo);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioRiskPolicyResponse>.Success(
            PortfolioRiskPolicyResponse.FromDomain(policy));
    }
}
