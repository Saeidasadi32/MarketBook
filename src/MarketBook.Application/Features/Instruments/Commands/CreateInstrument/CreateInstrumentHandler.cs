// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Commands.CreateInstrument
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

namespace MarketBook.Application.Features.Instruments.Commands.CreateInstrument;

/// <summary>
/// EN: Handles creating an Instrument aggregate.
/// FA: ایجاد Aggregate ابزار مالی را مدیریت می‌کند.
/// </summary>
public sealed class CreateInstrumentHandler
    : IRequestHandler<CreateInstrumentCommand, Result<InstrumentId>>
{
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the create instrument handler.
    /// FA: Handler ایجاد ابزار مالی را مقداردهی می‌کند.
    /// </summary>
    public CreateInstrumentHandler(
        IInstrumentRepository instrumentRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(instrumentRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _instrumentRepository = instrumentRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the create instrument command.
    /// FA: فرمان ایجاد ابزار مالی را پردازش می‌کند.
    /// </summary>
    public async Task<Result<InstrumentId>> Handle(
        CreateInstrumentCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

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
                    cancellationToken))
            {
                return Result<InstrumentId>.Fail(
                    new Error(
                        "Instrument.DuplicateIsin",
                        "An instrument with the specified ISIN already exists."));
            }
        }

        Instrument instrument = Instrument.Create(
            name,
            assetClass,
            (InstrumentType)request.Type,
            category,
            isin);

        await _instrumentRepository.AddAsync(
            instrument,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<InstrumentId>.Success(instrument.Id);
    }
}
