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
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: EF Core repository for immutable portfolio transactions.
/// FA: Repository مبتنی بر EF Core برای تراکنش‌های تغییرناپذیر پرتفوی.
/// </summary>
public sealed class PortfolioTransactionRepository
    : IPortfolioTransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PortfolioTransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PortfolioTransaction?> GetByIdAsync(
        PortfolioTransactionId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioTransaction>()
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    public async Task AddAsync(
        PortfolioTransaction transaction,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<PortfolioTransaction>()
            .AddAsync(transaction, cancellationToken);

    public async Task<PagedResult<PortfolioTransaction>> GetPagedAsync(
        PortfolioId portfolioId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<PortfolioTransaction> query =
            _dbContext.Set<PortfolioTransaction>()
                .AsNoTracking()
                .Where(item => item.PortfolioId == portfolioId)
                .OrderByDescending(item => item.ExecutedOn)
                .ThenByDescending(item => item.Id);

        int totalCount = await query.CountAsync(cancellationToken);

        List<PortfolioTransaction> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PortfolioTransaction>(
            items,
            page,
            pageSize,
            totalCount);
    }

    /// <inheritdoc/>
    public Task<List<PortfolioTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        ListingId? listingId,
        CancellationToken cancellationToken = default)
        => GetLedgerCoreAsync(
            portfolioId,
            listingId,
            null,
            cancellationToken);

    /// <inheritdoc/>
    public Task<List<PortfolioTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        ListingId? listingId,
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default)
        => GetLedgerCoreAsync(
            portfolioId,
            listingId,
            asOf,
            cancellationToken);

    private Task<List<PortfolioTransaction>> GetLedgerCoreAsync(
        PortfolioId portfolioId,
        ListingId? listingId,
        DateTimeOffset? asOf,
        CancellationToken cancellationToken)
    {
        IQueryable<PortfolioTransaction> query =
            _dbContext.Set<PortfolioTransaction>()
                .AsNoTracking()
                .Where(item => item.PortfolioId == portfolioId);

        if (listingId is not null)
        {
            query = query.Where(item => item.ListingId == listingId);
        }

        if (asOf.HasValue)
        {
            DateTimeOffset cutoff = asOf.Value;
            query = query.Where(item => item.ExecutedOn <= cutoff);
        }

        return query
            .OrderBy(item => item.ExecutedOn)
            .ThenBy(item => item.Id)
            .ToListAsync(cancellationToken);
    }
}
