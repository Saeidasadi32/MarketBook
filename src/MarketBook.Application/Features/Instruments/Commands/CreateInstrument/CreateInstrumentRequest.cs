// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.CreateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.Instruments.Commands.CreateInstrument;

/// <summary>
/// EN: Represents the request to create an instrument.
/// FA: درخواست ایجاد ابزار مالی را نشان می‌دهد.
/// </summary>
public sealed record CreateInstrumentRequest(
    string Name,
    string AssetClass,
    int Type,
    string Category,
    string? Isin);
