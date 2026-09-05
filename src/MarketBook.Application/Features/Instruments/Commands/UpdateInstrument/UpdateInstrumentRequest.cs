// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.UpdateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Instruments.Commands.UpdateInstrument;

/// <summary>
/// EN: Represents the request to update an instrument.
/// FA: درخواست به‌روزرسانی ابزار مالی را نشان می‌دهد.
/// </summary>
public sealed record UpdateInstrumentRequest(
    string Name,
    string AssetClass,
    int Type,
    string Category,
    string? Isin);
