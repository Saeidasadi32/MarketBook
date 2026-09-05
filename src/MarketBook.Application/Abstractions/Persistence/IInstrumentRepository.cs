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
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;

namespace MarketBook.Application.Abstractions.Persistence;

/// <summary>
/// EN: Defines persistence operations for the Instrument aggregate.
/// FA: عملیات ذخیره‌سازی Aggregate ابزار مالی را تعریف می‌کند.
/// </summary>
public interface IInstrumentRepository
{
    /// <summary>
    /// EN: Gets an instrument by identifier.
    /// FA: ابزار مالی را بر اساس شناسه دریافت می‌کند.
    /// </summary>
    Task<Instrument?> GetByIdAsync(
        InstrumentId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Gets an instrument by ISIN.
    /// FA: ابزار مالی را بر اساس ISIN دریافت می‌کند.
    /// </summary>
    Task<Instrument?> GetByIsinAsync(
        Isin isin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether an instrument with the specified ISIN exists.
    /// FA: بررسی می‌کند آیا ابزاری با ISIN مشخص وجود دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        Isin isin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Determines whether another instrument with the specified ISIN exists.
    /// FA: بررسی می‌کند آیا ابزار دیگری با ISIN مشخص وجود دارد یا خیر.
    /// </summary>
    Task<bool> ExistsAsync(
        Isin isin,
        InstrumentId excludingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Adds a new instrument.
    /// FA: ابزار مالی جدید را اضافه می‌کند.
    /// </summary>
    Task AddAsync(
        Instrument instrument,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// EN: Marks an instrument as modified.
    /// FA: ابزار مالی را به‌عنوان تغییرکرده علامت‌گذاری می‌کند.
    /// </summary>
    void Update(Instrument instrument);

    /// <summary>
    /// EN: Gets a paged collection of instruments.
    /// FA: مجموعه صفحه‌بندی‌شده ابزارهای مالی را دریافت می‌کند.
    /// </summary>
    Task<PagedResult<Instrument>> GetPagedAsync(
        PageRequest pageRequest,
        CancellationToken cancellationToken = default);
}
