// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Queries.GetAllInstruments
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Instruments.Queries.GetAllInstruments;

/// <summary>
/// EN: Represents a paged instrument response.
/// FA: پاسخ صفحه‌بندی‌شده ابزارهای مالی را نشان می‌دهد.
/// </summary>
public sealed record GetAllInstrumentsResponse(
    IReadOnlyCollection<InstrumentListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>
/// EN: Represents an instrument list item.
/// FA: یک آیتم از فهرست ابزارهای مالی را نشان می‌دهد.
/// </summary>
public sealed record InstrumentListItemResponse(
    string Id,
    string Name,
    string AssetClass,
    int Type,
    string Category,
    string? Isin,
    DateTimeOffset CreatedOn,
    bool IsActive);
