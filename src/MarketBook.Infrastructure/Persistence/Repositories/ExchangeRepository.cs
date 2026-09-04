// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Provides persistence operations for the Exchange aggregate.
/// FA: عملیات ماندگاری Aggregate بورس را فراهم می‌کند.
/// </summary>
internal sealed class ExchangeRepository : IExchangeRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="ExchangeRepository"/> class.
    /// FA: نمونه جدیدی از کلاس <see cref="ExchangeRepository"/> را ایجاد می‌کند.
    /// </summary>
    /// <param name="context">
    /// EN: Application database context.
    /// FA: کانتکست پایگاه داده برنامه.
    /// </param>
    public ExchangeRepository(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    /// <inheritdoc />
    public Task<Exchange?> GetByIdAsync(
        ExchangeId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return _context.Set<Exchange>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                exchange => exchange.Id == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<Exchange?> GetByCodeAsync(
        ExchangeCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return _context.Set<Exchange>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                exchange => exchange.Code == code,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        ExchangeCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return _context.Set<Exchange>()
            .AnyAsync(
                exchange => exchange.Code == code,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
    ExchangeCode code,
    ExchangeId excludingId,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(excludingId);

        return _context.Set<Exchange>()
            .AnyAsync(
                exchange =>
                    exchange.Code == code &&
                    exchange.Id != excludingId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        Exchange exchange,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        await _context.Set<Exchange>()
            .AddAsync(
                exchange,
                cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Exchange exchange)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        _context.Set<Exchange>()
            .Update(exchange);
    }

    /// <inheritdoc />
    public void Remove(Exchange exchange)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        _context.Set<Exchange>()
            .Remove(exchange);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Exchange>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int normalizedPage = pageRequest.NormalizedPage;
        int normalizedPageSize = pageRequest.NormalizedPageSize;

        IQueryable<Exchange> query = _context.Set<Exchange>()
            .AsNoTracking()
            .OrderBy(exchange => exchange.Name);

        int totalCount = await query.CountAsync(
            cancellationToken);

        List<Exchange> items = await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<Exchange>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount);
    }
}
