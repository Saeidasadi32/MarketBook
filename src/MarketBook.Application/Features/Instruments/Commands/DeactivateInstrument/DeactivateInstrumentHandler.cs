// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.DeactivateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Commands.DeactivateInstrument;

/// <summary>
/// EN: Handles deactivateing an Instrument aggregate.
/// FA: غیرفعال‌سازی Aggregate ابزار مالی را مدیریت می‌کند.
/// </summary>
public sealed class DeactivateInstrumentHandler
    : IRequestHandler<DeactivateInstrumentCommand, Result<InstrumentId>>
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public DeactivateInstrumentHandler(
        IInstrumentRepository instrumentRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(instrumentRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _instrumentRepository = instrumentRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the command.
    /// FA: فرمان را پردازش می‌کند.
    /// </summary>
    public async Task<Result<InstrumentId>> Handle(
        DeactivateInstrumentCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!InstrumentId.TryParse(
                request.Id,
                out InstrumentId? instrumentId) ||
            instrumentId is null)
        {
            return Result<InstrumentId>.Fail(
                new Error(
                    "Instrument.InvalidId",
                    "The specified instrument identifier is invalid."));
        }

        Instrument? instrument =
            await _instrumentRepository.GetByIdAsync(
                instrumentId,
                cancellationToken);

        if (instrument is null)
        {
            return Result<InstrumentId>.Fail(
                new Error(
                    "Instrument.NotFound",
                    "The specified instrument was not found."));
        }

        instrument.Deactivate();
        _instrumentRepository.Update(instrument);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<InstrumentId>.Success(instrument.Id);
    }
}
