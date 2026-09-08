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
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Provides EF persistence for portfolios.
/// FA: ماندگاری EF برای پرتفوی‌ها را فراهم می‌کند.
/// </summary>
public sealed class PortfolioRepository : IPortfolioRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the repository.
    /// FA: Repository را مقداردهی می‌کند.
    /// </summary>
    public PortfolioRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<Portfolio?> GetByIdAsync(
        PortfolioId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<Portfolio>()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsByInvestorAndNameAsync(
        InvestorId investorId,
        PortfolioName name,
        PortfolioId? excludingPortfolioId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Portfolio> query = _dbContext.Set<Portfolio>()
            .AsNoTracking()
            .Where(item =>
                item.InvestorId == investorId &&
                item.Name == name);

        if (excludingPortfolioId is not null)
        {
            query = query.Where(item => item.Id != excludingPortfolioId);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        Portfolio portfolio,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<Portfolio>()
            .AddAsync(portfolio, cancellationToken);

    /// <inheritdoc />
    public void Update(Portfolio portfolio)
        => _dbContext.Set<Portfolio>().Update(portfolio);

    /// <inheritdoc />
    public async Task<PagedResult<Portfolio>> GetPagedAsync(
        PageRequest pageRequest,
        InvestorId? investorId,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<Portfolio> query = _dbContext.Set<Portfolio>()
            .AsNoTracking();

        if (investorId is not null)
        {
            query = query.Where(item => item.InvestorId == investorId);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Portfolio> items = await query
            .OrderBy(item => item.Name)
            .ThenBy(item => item.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Portfolio>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
