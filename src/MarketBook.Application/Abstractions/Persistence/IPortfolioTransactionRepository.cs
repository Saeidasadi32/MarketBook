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
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Persistence contract for immutable portfolio transactions.
/// FA: قرارداد ماندگاری تراکنش‌های تغییرناپذیر پرتفوی.
/// </summary>
public interface IPortfolioTransactionRepository
{
    /// <summary>
    /// EN: Gets one portfolio transaction by identifier.
    /// FA: یک تراکنش پرتفوی را با شناسه دریافت می‌کند.
    /// </summary>
    Task<PortfolioTransaction?> GetByIdAsync(
        PortfolioTransactionId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds one immutable transaction to the persistence context.
    /// FA: یک تراکنش تغییرناپذیر را به context ماندگاری اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        PortfolioTransaction transaction,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets a paged transaction list for one portfolio.
    /// FA: فهرست صفحه‌بندی‌شده تراکنش‌های یک پرتفوی را دریافت می‌کند.
    /// </summary>
    Task<PagedResult<PortfolioTransaction>> GetPagedAsync(
        PortfolioId portfolioId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets the ordered transaction ledger for a portfolio, optionally filtered by listing.
    /// FA: دفتر مرتب‌شده تراکنش‌های پرتفوی را با فیلتر اختیاری Listing دریافت می‌کند.
    /// </summary>
    Task<List<PortfolioTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        ListingId? listingId,
        CancellationToken cancellationToken = default);
}
