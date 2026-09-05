// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Queries.GetInstrumentById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Instruments.Queries.GetInstrumentById;

/// <summary>
/// EN: Represents an instrument details response.
/// FA: پاسخ جزئیات ابزار مالی را نشان می‌دهد.
/// </summary>
public sealed record GetInstrumentByIdResponse(
    string Id,
    string Name,
    string AssetClass,
    int Type,
    string Category,
    string? Isin,
    DateTimeOffset CreatedOn,
    bool IsActive);
