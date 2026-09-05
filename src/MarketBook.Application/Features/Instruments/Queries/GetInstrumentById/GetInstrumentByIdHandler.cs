// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Queries.GetInstrumentById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Queries.GetInstrumentById;

/// <summary>
/// EN: Handles retrieving an instrument by identifier.
/// FA: دریافت ابزار مالی بر اساس شناسه را مدیریت می‌کند.
/// </summary>
public sealed class GetInstrumentByIdHandler
    : IRequestHandler<GetInstrumentByIdQuery, Result<GetInstrumentByIdResponse>>
{
    private readonly IInstrumentRepository _instrumentRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetInstrumentByIdHandler(
        IInstrumentRepository instrumentRepository)
    {
        ArgumentNullException.ThrowIfNull(instrumentRepository);
        _instrumentRepository = instrumentRepository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetInstrumentByIdResponse>> Handle(
        GetInstrumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!InstrumentId.TryParse(
                request.Id,
                out InstrumentId? instrumentId) ||
            instrumentId is null)
        {
            return Result<GetInstrumentByIdResponse>.Fail(
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
            return Result<GetInstrumentByIdResponse>.Fail(
                new Error(
                    "Instrument.NotFound",
                    "The specified instrument was not found."));
        }

        GetInstrumentByIdResponse response = new(
            instrument.Id.Value.ToString(),
            instrument.Name.Value,
            instrument.AssetClass.Value,
            (int)instrument.Type,
            instrument.Category.Value,
            instrument.Isin?.Value,
            instrument.CreatedOn,
            instrument.IsActive);

        return Result<GetInstrumentByIdResponse>.Success(response);
    }
}
