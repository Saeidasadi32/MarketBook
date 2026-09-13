// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskPolicy.Aggregates;
using MarketBook.Domain.PortfolioRiskPolicy.Enums;
using MarketBook.Domain.PortfolioRiskPolicy.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>EN: Provides EF persistence for portfolio risk policies. FA: ماندگاری EF سیاست‌های ریسک پرتفوی را فراهم می‌کند.</summary>
public sealed class PortfolioRiskPolicyRepository : IPortfolioRiskPolicyRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the repository. FA: Repository را مقداردهی می‌کند.</summary>
    public PortfolioRiskPolicyRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<PortfolioRiskPolicy?> GetActiveAsync(PortfolioId portfolioId, CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioRiskPolicy>()
            .SingleOrDefaultAsync(
                item => item.PortfolioId == portfolioId && item.Status == RiskPolicyStatus.Active,
                cancellationToken);

    /// <inheritdoc />
    public Task<PortfolioRiskPolicy?> GetByIdAsync(PortfolioRiskPolicyId id, CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioRiskPolicy>().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<PortfolioRiskPolicy?> GetByVersionAsync(
        PortfolioId portfolioId,
        int policyVersion,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioRiskPolicy>().SingleOrDefaultAsync(
            item => item.PortfolioId == portfolioId && item.PolicyVersion == policyVersion,
            cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PortfolioRiskPolicy>> GetHistoryAsync(
        PortfolioId portfolioId,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<PortfolioRiskPolicy>()
            .AsNoTracking()
            .Where(item => item.PortfolioId == portfolioId)
            .OrderBy(item => item.PolicyVersion)
            .ToArrayAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> GetNextVersionAsync(PortfolioId portfolioId, CancellationToken cancellationToken = default)
    {
        int? maximum = await _dbContext.Set<PortfolioRiskPolicy>()
            .Where(item => item.PortfolioId == portfolioId)
            .Select(item => (int?)item.PolicyVersion)
            .MaxAsync(cancellationToken);

        return (maximum ?? 0) + 1;
    }

    /// <inheritdoc />
    public async Task AddAsync(PortfolioRiskPolicy policy, CancellationToken cancellationToken = default)
        => await _dbContext.Set<PortfolioRiskPolicy>().AddAsync(policy, cancellationToken);
}
