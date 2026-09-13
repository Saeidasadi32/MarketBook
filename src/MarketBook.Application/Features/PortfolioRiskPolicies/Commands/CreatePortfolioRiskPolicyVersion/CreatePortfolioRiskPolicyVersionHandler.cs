// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicyVersion
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MarketBook.Domain.PortfolioRiskPolicy.Enums;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicyVersion;

/// <summary>EN: Handles creation of the next draft version. FA: ایجاد نسخه Draft بعدی را مدیریت می‌کند.</summary>
public sealed class CreatePortfolioRiskPolicyVersionHandler
    : IRequestHandler<CreatePortfolioRiskPolicyVersionCommand, Result<PortfolioRiskPolicyResponse>>
{
    private readonly IPortfolioRiskPolicyRepository _policies;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public CreatePortfolioRiskPolicyVersionHandler(
        IPortfolioRiskPolicyRepository policies,
        IApplicationDbContext dbContext)
    {
        _policies = policies;
        _dbContext = dbContext;
    }

    /// <summary>EN: Creates the next version as Draft. FA: نسخه بعدی را به‌صورت Draft ایجاد می‌کند.</summary>
    public async Task<Result<PortfolioRiskPolicyResponse>> Handle(
        CreatePortfolioRiskPolicyVersionCommand request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        int nextVersion = await _policies.GetNextVersionAsync(portfolioId, cancellationToken);
        if (nextVersion == 1)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.NotInitialized", "Create the first active risk policy before creating revisions."));
        }

        PortfolioRiskPolicyLimitsRequest limits = request.Limits;
        PortfolioRiskPolicy policy = new(
            PortfolioRiskPolicyId.New(),
            portfolioId,
            nextVersion,
            limits.EffectiveFrom,
            RiskPolicyStatus.Draft,
            limits.MaxAnnualizedVolatility,
            limits.MaxValueAtRiskReturn,
            limits.MaxValueAtRiskAmountBase,
            limits.MaxDrawdownLossRatio,
            limits.MaxDrawdownAmountBase,
            limits.MinSharpeRatio,
            limits.MinSortinoRatio);

        await _policies.AddAsync(policy, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<PortfolioRiskPolicyResponse>.Success(
            PortfolioRiskPolicyResponse.FromDomain(policy));
    }
}
