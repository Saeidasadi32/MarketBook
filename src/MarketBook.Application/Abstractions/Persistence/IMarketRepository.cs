using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for the Market aggregate.
/// FA: عملیات ماندگاری داده برای Aggregate مربوط به بازار را تعریف می‌کند.
/// </summary>
public interface IMarketRepository
{
    /// <summary>
    /// EN: Retrieves a market by its identifier.
    /// FA: یک بازار را بر اساس شناسه آن بازیابی می‌کند.
    /// </summary>
    /// <param name="id">
    /// EN: Market identifier.
    /// FA: شناسه بازار.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The market if found; otherwise null.
    /// FA: بازار در صورت یافت شدن؛ در غیر این صورت null.
    /// </returns>
    Task<Market?> GetByIdAsync(
        MarketId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Retrieves a market by its unique code.
    /// FA: یک بازار را بر اساس کد یکتای آن بازیابی می‌کند.
    /// </summary>
    /// <param name="code">
    /// EN: Market code.
    /// FA: کد بازار.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The market if found; otherwise null.
    /// FA: بازار در صورت یافت شدن؛ در غیر این صورت null.
    /// </returns>
    Task<Market?> GetByCodeAsync(
        MarketCode code,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// EN: Determines whether a Market with the specified code exists.
    /// FA: بررسی می‌کند آیا بازاری با کد مشخص‌شده وجود دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        MarketCode code,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether a market with the specified code exists.
    /// FA: مشخص می‌کند آیا بازاری با کد مشخص‌شده وجود دارد یا خیر.
    /// </summary>
    /// <param name="code">
    /// EN: Market code.
    /// FA: کد بازار.
    /// </param>
    /// <param name="excludingId"></param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: True if the market exists; otherwise false.
    /// FA: در صورت وجود بازار true و در غیر این صورت false.
    /// </returns>
    Task<bool> ExistsAsync(
        MarketCode code,
        MarketId excludingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a new market to the persistence context.
    /// FA: یک بازار جدید را به Context ماندگاری اضافه می‌کند.
    /// </summary>
    /// <param name="market">
    /// EN: Market to add.
    /// FA: بازاری که باید اضافه شود.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    Task AddAsync(
        Market market,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks a market as modified in the persistence context.
    /// FA: یک بازار را به عنوان تغییر‌یافته در Context ماندگاری علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="market">
    /// EN: Market to update.
    /// FA: بازاری که باید به‌روزرسانی شود.
    /// </param>
    void Update(Market market);

    /// <summary>
    /// EN: Marks a market for removal from persistence.
    /// FA: یک بازار را برای حذف از ماندگاری علامت‌گذاری می‌کند.
    /// </summary>
    /// <param name="market">
    /// EN: Market to remove.
    /// FA: بازاری که باید حذف شود.
    /// </param>
    void Remove(Market market);

    /// <summary>
    /// EN: Retrieves markets using pagination.
    /// FA: بازارها را به صورت صفحه‌بندی‌شده بازیابی می‌کند.
    /// </summary>
    /// <param name="pageRequest">
    /// EN: Pagination parameters.
    /// FA: پارامترهای صفحه‌بندی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: A paged result containing markets.
    /// FA: نتیجه صفحه‌بندی‌شده شامل بازارها.
    /// </returns>
    Task<PagedResult<Market>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
