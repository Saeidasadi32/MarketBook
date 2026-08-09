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
/// EN: Represents pagination parameters for a paged query.
/// FA: پارامترهای صفحه‌بندی برای یک پرس‌وجوی صفحه‌بندی‌شده.
/// </summary>
public sealed record PageRequest
{
    /// <summary>
    /// EN: Default page number.
    /// FA: شماره صفحه پیش‌فرض.
    /// </summary>
    public const int DefaultPage = 1;

    /// <summary>
    /// EN: Default page size.
    /// FA: اندازه صفحه پیش‌فرض.
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// EN: Maximum allowed page size.
    /// FA: حداکثر اندازه مجاز صفحه.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// EN: Gets the requested page number.
    /// FA: شماره صفحه درخواستی.
    /// </summary>
    public int Page { get; init; } = DefaultPage;

    /// <summary>
    /// EN: Gets the requested page size.
    /// FA: اندازه صفحه درخواستی.
    /// </summary>
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>
    /// EN: Gets the normalized page number.
    /// FA: شماره صفحه نرمال‌شده.
    /// </summary>
    public int NormalizedPage =>
        Page < 1
            ? DefaultPage
            : Page;

    /// <summary>
    /// EN: Gets the normalized page size.
    /// FA: اندازه صفحه نرمال‌شده.
    /// </summary>
    public int NormalizedPageSize =>
        PageSize switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => PageSize
        };

    /// <summary>
    /// EN: Gets the number of records to skip.
    /// FA: تعداد رکوردهایی که باید نادیده گرفته شوند.
    /// </summary>
    public int Skip =>
        (NormalizedPage - 1) * NormalizedPageSize;
}
