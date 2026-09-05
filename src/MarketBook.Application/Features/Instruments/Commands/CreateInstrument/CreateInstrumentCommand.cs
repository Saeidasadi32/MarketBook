// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.CreateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Commands.CreateInstrument;

/// <summary>
/// EN: Represents a command to create an instrument.
/// FA: فرمان ایجاد ابزار مالی را نشان می‌دهد.
/// </summary>
public sealed record CreateInstrumentCommand(
    string Name,
    string AssetClass,
    int Type,
    string Category,
    string? Isin) : IRequest<Result<InstrumentId>>;
