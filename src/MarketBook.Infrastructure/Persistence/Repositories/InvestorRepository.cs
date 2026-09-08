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
using MarketBook.Domain.Investor.Aggregates;
using MarketBook.Domain.Investor.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>EN: Provides EF persistence for investors. FA: ماندگاری EF برای سرمایه‌گذاران را فراهم می‌کند.</summary>
public sealed class InvestorRepository : IInvestorRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the repository. FA: Repository را مقداردهی می‌کند.</summary>
    public InvestorRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<Investor?> GetByIdAsync(
        InvestorId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<Investor>()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(
        Investor investor,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<Investor>().AddAsync(investor, cancellationToken);

    /// <inheritdoc />
    public void Update(Investor investor)
        => _dbContext.Set<Investor>().Update(investor);

    /// <inheritdoc />
    public async Task<PagedResult<Investor>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<Investor> query = _dbContext.Set<Investor>()
            .AsNoTracking();

        int totalCount = await query.CountAsync(cancellationToken);

        List<Investor> items = await query
            .OrderBy(item => item.FullName)
            .ThenBy(item => item.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Investor>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
