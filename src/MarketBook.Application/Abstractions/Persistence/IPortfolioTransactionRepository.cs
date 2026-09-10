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
    /// EN: Gets a portfolio transaction by its identifier.
    /// FA: یک تراکنش پرتفوی را براساس شناسه آن دریافت می‌کند.
    /// </summary>
    /// <param name="id">EN: Transaction identifier. FA: شناسه تراکنش.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>
    /// EN: The transaction when found; otherwise null.
    /// FA: تراکنش در صورت وجود؛ در غیر این صورت null.
    /// </returns>
    Task<PortfolioTransaction?> GetByIdAsync(
        PortfolioTransactionId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a new immutable portfolio transaction.
    /// FA: یک تراکنش تغییرناپذیر جدید به پرتفوی اضافه می‌کند.
    /// </summary>
    /// <param name="transaction">EN: Transaction to add. FA: تراکنش موردنظر برای افزودن.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    Task AddAsync(
        PortfolioTransaction transaction,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets paged portfolio transactions in deterministic order.
    /// FA: تراکنش‌های پرتفوی را به‌صورت صفحه‌بندی‌شده و با ترتیب قطعی دریافت می‌کند.
    /// </summary>
    /// <param name="portfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
    /// <param name="pageRequest">EN: Paging parameters. FA: پارامترهای صفحه‌بندی.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>
    /// EN: A paged collection of portfolio transactions.
    /// FA: مجموعه صفحه‌بندی‌شده تراکنش‌های پرتفوی.
    /// </returns>
    Task<PagedResult<PortfolioTransaction>> GetPagedAsync(
        PortfolioId portfolioId,
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets the deterministic transaction ledger for a portfolio, optionally filtered by listing.
    /// FA: دفتر قطعی تراکنش‌های پرتفوی را با امکان فیلتر اختیاری براساس Listing دریافت می‌کند.
    /// </summary>
    /// <param name="portfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
    /// <param name="listingId">EN: Optional listing identifier. FA: شناسه اختیاری Listing.</param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>
    /// EN: Ordered immutable transaction ledger.
    /// FA: دفتر مرتب و تغییرناپذیر تراکنش‌ها.
    /// </returns>
    Task<List<PortfolioTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        ListingId? listingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets the deterministic portfolio ledger limited to an inclusive as-of instant.
    /// FA: دفتر قطعی پرتفوی را تا یک لحظه تاریخی شامل‌شونده دریافت می‌کند.
    /// </summary>
    /// <param name="portfolioId">EN: Portfolio identifier. FA: شناسه پرتفوی.</param>
    /// <param name="listingId">EN: Optional listing identifier. FA: شناسه اختیاری Listing.</param>
    /// <param name="asOf">
    /// EN: Inclusive historical cutoff instant.
    /// FA: لحظه تاریخی شامل‌شونده برای برش Ledger.
    /// </param>
    /// <param name="cancellationToken">EN: Cancellation token. FA: توکن لغو.</param>
    /// <returns>
    /// EN: Ordered ledger containing only transactions executed on or before the cutoff.
    /// FA: دفتر مرتب شامل فقط تراکنش‌هایی که در لحظه برش یا قبل از آن اجرا شده‌اند.
    /// </returns>
    Task<List<PortfolioTransaction>> GetLedgerAsync(
        PortfolioId portfolioId,
        ListingId? listingId,
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default);
}
