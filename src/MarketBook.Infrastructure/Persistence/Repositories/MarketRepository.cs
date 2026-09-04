using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Provides Entity Framework Core persistence operations for the Market aggregate.
/// FA: عملیات ماندگاری Entity Framework Core برای Aggregate مربوط به بازار را فراهم می‌کند.
/// </summary>
public sealed class MarketRepository : IMarketRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the Market repository.
    /// FA: یک نمونه جدید از Repository بازار را ایجاد می‌کند.
    /// </summary>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public MarketRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Market?> GetByIdAsync(
        MarketId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        return await _dbContext.Set<Market>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                market => market.Id == id,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Market?> GetByCodeAsync(
        MarketCode code,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _dbContext.Set<Market>()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                market => market.Code == code,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
    MarketCode code,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        return await _dbContext.Set<Market>()
            .AnyAsync(
                market => market.Code == code,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        MarketCode code,
        MarketId excludingId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(excludingId);

        return await _dbContext.Set<Market>()
            .AnyAsync(
                market =>
                    market.Code == code &&
                    market.Id != excludingId,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        Market market,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(market);

        await _dbContext.Set<Market>().AddAsync(
            market,
            cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Market market)
    {
        ArgumentNullException.ThrowIfNull(market);

        _dbContext.Set<Market>().Update(market);
    }

    /// <inheritdoc />
    public void Remove(Market market)
    {
        ArgumentNullException.ThrowIfNull(market);

        _dbContext.Set<Market>().Remove(market);
    }

    /// <inheritdoc />
    public async Task<PagedResult<Market>> GetPagedAsync(
            PageRequest pageRequest,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);

        int normalizedPage = pageRequest.NormalizedPage;
        int normalizedPageSize = pageRequest.NormalizedPageSize;

        IQueryable<Market> query = _dbContext.Set<Market>()
            .AsNoTracking()
            .OrderBy(market => market.Code);

        int totalCount =
            await query.CountAsync(cancellationToken);

        List<Market> items =
            await query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToListAsync(cancellationToken);

        return new PagedResult<Market>(
            items,
            normalizedPage,
            normalizedPageSize,
            totalCount);
    }
}
