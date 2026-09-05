// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.ActivateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Commands.ActivateInstrument;

/// <summary>
/// EN: Represents a command to activate an instrument.
/// FA: فرمان فعال‌سازی ابزار مالی را نشان می‌دهد.
/// </summary>
public sealed record ActivateInstrumentCommand(
    string Id) : IRequest<Result<InstrumentId>>;
