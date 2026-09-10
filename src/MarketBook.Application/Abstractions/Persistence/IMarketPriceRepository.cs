// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Persistence
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.MarketData.Aggregates;
using MarketBook.Domain.MarketData.ValueObjects;
namespace MarketBook.Application.Abstractions.Persistence;
/// <summary>EN: Defines MarketPrice persistence operations. FA: عملیات ماندگاری MarketPrice را تعریف می‌کند.</summary>
public interface IMarketPriceRepository
{
/// <summary>EN: Gets a record by identifier. FA: رکورد را بر اساس شناسه دریافت می‌کند.</summary>
Task<MarketPrice?> GetByIdAsync(MarketPriceId id,CancellationToken cancellationToken=default);
/// <summary>EN: Checks Listing/date uniqueness. FA: یکتایی Listing و تاریخ را بررسی می‌کند.</summary>
Task<bool> ExistsAsync(ListingId listingId,DateOnly tradingDate,CancellationToken cancellationToken=default);
/// <summary>EN: Adds a record. FA: رکورد را اضافه می‌کند.</summary>
Task AddAsync(MarketPrice marketPrice,CancellationToken cancellationToken=default);
/// <summary>EN: Marks a record modified. FA: رکورد را تغییرکرده علامت می‌زند.</summary>
void Update(MarketPrice marketPrice);

/// <summary>EN: Gets the latest available market price for a Listing. FA: آخرین قیمت بازار موجود برای یک Listing را دریافت می‌کند.</summary>
Task<MarketPrice?> GetLatestByListingIdAsync(ListingId listingId,CancellationToken cancellationToken=default);
/// <summary>EN: Gets the latest market price on or before a trading date. FA: آخرین قیمت بازار را در تاریخ معاملاتی تعیین‌شده یا قبل از آن دریافت می‌کند.</summary>
Task<MarketPrice?> GetLatestByListingIdAsync(ListingId listingId,DateOnly onOrBeforeTradingDate,CancellationToken cancellationToken=default);
/// <summary>EN: Gets paged records. FA: رکوردهای صفحه‌بندی‌شده را دریافت می‌کند.</summary>
Task<PagedResult<MarketPrice>> GetPagedAsync(PageRequest pageRequest,CancellationToken cancellationToken=default);
}
