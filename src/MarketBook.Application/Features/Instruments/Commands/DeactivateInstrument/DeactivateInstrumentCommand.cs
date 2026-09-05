// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.DeactivateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Commands.DeactivateInstrument;

/// <summary>
/// EN: Represents a command to deactivate an instrument.
/// FA: فرمان غیرفعال‌سازی ابزار مالی را نشان می‌دهد.
/// </summary>
public sealed record DeactivateInstrumentCommand(
    string Id) : IRequest<Result<InstrumentId>>;
