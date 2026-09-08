// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence; using MarketBook.Application.Common.Pagination; using MarketBook.Domain.Listing.ValueObjects; using MarketBook.Domain.MarketData.Aggregates; using MarketBook.Domain.MarketData.ValueObjects; using MarketBook.Infrastructure.Persistence.Context; using Microsoft.EntityFrameworkCore; namespace MarketBook.Infrastructure.Persistence.Repositories;
/// <summary>EN: EF repository for MarketPrice. FA: Repository مبتنی بر EF برای MarketPrice.</summary>
public sealed class MarketPriceRepository:IMarketPriceRepository{private readonly ApplicationDbContext _dbContext;
/// <summary>EN: Initializes repository. FA: Repository را مقداردهی می‌کند.</summary>
public MarketPriceRepository(ApplicationDbContext dbContext){ArgumentNullException.ThrowIfNull(dbContext);_dbContext=dbContext;}
/// <inheritdoc/>
public Task<MarketPrice?> GetByIdAsync(MarketPriceId id,CancellationToken cancellationToken=default)=>_dbContext.Set<MarketPrice>().SingleOrDefaultAsync(item=>item.Id==id,cancellationToken);
/// <inheritdoc/>
public Task<bool> ExistsAsync(ListingId listingId,DateOnly tradingDate,CancellationToken cancellationToken=default)=>_dbContext.Set<MarketPrice>().AnyAsync(item=>item.ListingId==listingId && item.TradingDate==tradingDate,cancellationToken);
/// <inheritdoc/>
public async Task AddAsync(MarketPrice marketPrice,CancellationToken cancellationToken=default)=>await _dbContext.Set<MarketPrice>().AddAsync(marketPrice,cancellationToken);
/// <inheritdoc/>
public void Update(MarketPrice marketPrice)=>_dbContext.Set<MarketPrice>().Update(marketPrice);
/// <inheritdoc/>
public async Task<PagedResult<MarketPrice>> GetPagedAsync(PageRequest pageRequest,CancellationToken cancellationToken=default){int page=pageRequest.NormalizedPage;int pageSize=pageRequest.NormalizedPageSize;IQueryable<MarketPrice> query=_dbContext.Set<MarketPrice>().AsNoTracking().OrderByDescending(item=>item.TradingDate).ThenByDescending(item=>item.CreatedOn);int total=await query.CountAsync(cancellationToken);List<MarketPrice> items=await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync(cancellationToken);return new PagedResult<MarketPrice>(items,page,pageSize,total);}}
