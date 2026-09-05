// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.UpdateInstrument
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.Enums;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Commands.UpdateInstrument;

/// <summary>
/// EN: Handles updating an Instrument aggregate.
/// FA: به‌روزرسانی Aggregate ابزار مالی را مدیریت می‌کند.
/// </summary>
public sealed class UpdateInstrumentHandler
    : IRequestHandler<UpdateInstrumentCommand, Result<InstrumentId>>
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the update instrument handler.
    /// FA: Handler به‌روزرسانی ابزار مالی را مقداردهی می‌کند.
    /// </summary>
    public UpdateInstrumentHandler(
        IInstrumentRepository instrumentRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(instrumentRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _instrumentRepository = instrumentRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the update instrument command.
    /// FA: فرمان به‌روزرسانی ابزار مالی را پردازش می‌کند.
    /// </summary>
    public async Task<Result<InstrumentId>> Handle(
        UpdateInstrumentCommand request,
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

        InstrumentName name;
        AssetClass assetClass;
        InstrumentCategory category;
        Isin? isin = null;

        try
        {
            name = new InstrumentName(request.Name);
        }
        catch (ArgumentException)
        {
            return Result<InstrumentId>.Fail(
                new Error(
                    "Instrument.InvalidName",
                    "The specified instrument name is invalid."));
        }

        try
        {
            assetClass = new AssetClass(request.AssetClass);
        }
        catch (ArgumentException)
        {
            return Result<InstrumentId>.Fail(
                new Error(
                    "Instrument.InvalidAssetClass",
                    "The specified asset class is invalid."));
        }

        if (!Enum.IsDefined(typeof(InstrumentType), request.Type))
        {
            return Result<InstrumentId>.Fail(
                new Error(
                    "Instrument.InvalidType",
                    "The specified instrument type is invalid."));
        }

        try
        {
            category = new InstrumentCategory(request.Category);
        }
        catch (ArgumentException)
        {
            return Result<InstrumentId>.Fail(
                new Error(
                    "Instrument.InvalidCategory",
                    "The specified instrument category is invalid."));
        }

        if (!string.IsNullOrWhiteSpace(request.Isin))
        {
            try
            {
                isin = new Isin(request.Isin);
            }
            catch (ArgumentException)
            {
                return Result<InstrumentId>.Fail(
                    new Error(
                        "Instrument.InvalidIsin",
                        "The specified ISIN is invalid."));
            }

            if (await _instrumentRepository.ExistsAsync(
                    isin,
                    instrumentId,
                    cancellationToken))
            {
                return Result<InstrumentId>.Fail(
                    new Error(
                        "Instrument.DuplicateIsin",
                        "An instrument with the specified ISIN already exists."));
            }
        }

        instrument.Rename(name);
        instrument.ChangeAssetClass(assetClass);
        instrument.ChangeType((InstrumentType)request.Type);
        instrument.ChangeCategory(category);

        if (isin is null)
        {
            instrument.RemoveIsin();
        }
        else
        {
            instrument.SetIsin(isin);
        }

        _instrumentRepository.Update(instrument);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<InstrumentId>.Success(instrument.Id);
    }
}
