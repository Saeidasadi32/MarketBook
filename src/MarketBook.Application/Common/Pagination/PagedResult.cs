// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Common.Pagination
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Common.Pagination;

/// <summary>
/// EN: Represents a paginated result.
/// FA: نتیجه یک پرس‌وجوی صفحه‌بندی‌شده را نمایش می‌دهد.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed record PagedResult<T>
{
    /// <summary>
    /// EN: Initializes a paged result.
    /// FA: یک نتیجه صفحه‌بندی‌شده ایجاد می‌کند.
    /// </summary>
    public PagedResult(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);

        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);

        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// EN: Gets the items on the current page.
    /// FA: آیتم‌های صفحه جاری.
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// EN: Gets the current page number.
    /// FA: شماره صفحه جاری.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// EN: Gets the page size.
    /// FA: اندازه صفحه.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// EN: Gets the total number of records.
    /// FA: تعداد کل رکوردها.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// EN: Gets the total number of pages.
    /// FA: تعداد کل صفحات.
    /// </summary>
    public int TotalPages =>
        TotalCount == 0
            ? 0
            : (int)Math.Ceiling(
                TotalCount / (double)PageSize);

    /// <summary>
    /// EN: Indicates whether a previous page exists.
    /// FA: آیا صفحه قبلی وجود دارد؟
    /// </summary>
    public bool HasPreviousPage =>
        Page > 1;

    /// <summary>
    /// EN: Indicates whether a next page exists.
    /// FA: آیا صفحه بعدی وجود دارد؟
    /// </summary>
    public bool HasNextPage =>
        Page < TotalPages;
}
