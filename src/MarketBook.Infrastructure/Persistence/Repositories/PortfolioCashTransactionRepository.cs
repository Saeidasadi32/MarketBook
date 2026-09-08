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
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>EN: EF Core repository for immutable portfolio cash transactions. FA: Repository مبتنی بر EF Core برای تراکنش‌های تغییرناپذیر دفتر نقدی پرتفوی.</summary>
public sealed class PortfolioCashTransactionRepository : IPortfolioCashTransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the repository. FA: Repository را مقداردهی اولیه می‌کند.</summary>
    public PortfolioCashTransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>EN: Gets a cash transaction by id. FA: تراکنش نقدی را بر اساس شناسه دریافت می‌کند.</summary>
    public Task<PortfolioCashTransaction?> GetByIdAsync(
        PortfolioCashTransactionId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioCashTransaction>().AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <summary>EN: Appends a cash transaction. FA: تراکنش نقدی را اضافه می‌کند.</summary>
    public async Task AddAsync(
        PortfolioCashTransaction transaction,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<PortfolioCashTransaction>().AddAsync(transaction, cancellationToken);

    /// <summary>EN: Gets normalized paged cash transactions. FA: تراکنش‌های نقدی صفحه‌بندی‌شده و نرمال‌شده را دریافت می‌کند.</summary>
    public async Task<PagedResult<PortfolioCashTransaction>> GetPagedAsync(
        PortfolioId portfolioId,
        CurrencyId? currencyId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        int page = pageRequest.NormalizedPage;
        int pageSize = pageRequest.NormalizedPageSize;

        IQueryable<PortfolioCashTransaction> query = _dbContext.Set<PortfolioCashTransaction>()
            .AsNoTracking()
            .Where(item => item.PortfolioId == portfolioId);

        if (currencyId is not null)
            query = query.Where(item => item.CurrencyId == currencyId);

        query = query.OrderByDescending(item => item.OccurredOn).ThenByDescending(item => item.Id);
        int totalCount = await query.CountAsync(cancellationToken);
        List<PortfolioCashTransaction> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<PortfolioCashTransaction>(items, page, pageSize, totalCount);
    }

    /// <summary>EN: Gets ledger entries in deterministic chronological order. FA: سطرهای دفتر را با ترتیب زمانی قطعی دریافت می‌کند.</summary>
    public Task<List<PortfolioCashTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioCashTransaction>()
            .AsNoTracking()
            .Where(item => item.PortfolioId == portfolioId)
            .OrderBy(item => item.OccurredOn)
            .ThenBy(item => item.Id)
            .ToListAsync(cancellationToken);
}
