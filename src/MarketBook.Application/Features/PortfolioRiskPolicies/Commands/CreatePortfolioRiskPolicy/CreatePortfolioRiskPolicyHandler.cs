// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicy
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MarketBook.Domain.PortfolioRiskPolicy.Enums;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.PortfolioRiskPolicies.Commands.CreatePortfolioRiskPolicy;

/// <summary>EN: Handles first risk-policy creation. FA: ایجاد اولین Policy ریسک را مدیریت می‌کند.</summary>
public sealed class CreatePortfolioRiskPolicyHandler
    : IRequestHandler<CreatePortfolioRiskPolicyCommand, Result<PortfolioRiskPolicyResponse>>
{
    private readonly IPortfolioRepository _portfolios;
    private readonly IPortfolioRiskPolicyRepository _policies;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public CreatePortfolioRiskPolicyHandler(
        IPortfolioRepository portfolios,
        IPortfolioRiskPolicyRepository policies,
        IApplicationDbContext dbContext)
    {
        _portfolios = portfolios;
        _policies = policies;
        _dbContext = dbContext;
    }

    /// <summary>EN: Creates version one as active. FA: نسخه یک را فعال ایجاد می‌کند.</summary>
    public async Task<Result<PortfolioRiskPolicyResponse>> Handle(
        CreatePortfolioRiskPolicyCommand request,
        CancellationToken cancellationToken)
    {
        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        Portfolio? portfolio = await _portfolios.GetByIdAsync(portfolioId, cancellationToken);
        if (portfolio is null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.PortfolioNotFound", "The portfolio was not found."));
        }

        PortfolioRiskPolicy? active = await _policies.GetActiveAsync(portfolioId, cancellationToken);
        if (active is not null)
        {
            return Result<PortfolioRiskPolicyResponse>.Fail(
                new Error("PortfolioRiskPolicy.ActiveAlreadyExists", "An active risk policy already exists."));
        }

        PortfolioRiskPolicyLimitsRequest limits = request.Limits;
        PortfolioRiskPolicy policy = new(
            PortfolioRiskPolicyId.New(),
            portfolioId,
            1,
            limits.EffectiveFrom,
            RiskPolicyStatus.Active,
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
