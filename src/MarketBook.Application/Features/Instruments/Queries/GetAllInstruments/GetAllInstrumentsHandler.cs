// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Instruments.Queries.GetAllInstruments
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Common.Pagination;
using MarketBook.Domain.Common;
using MarketBook.Domain.Instrument.Aggregates;
using MediatR;

namespace MarketBook.Application.Features.Instruments.Queries.GetAllInstruments;

/// <summary>
/// EN: Handles retrieving a paged collection of instruments.
/// FA: دریافت مجموعه صفحه‌بندی‌شده ابزارهای مالی را مدیریت می‌کند.
/// </summary>
public sealed class GetAllInstrumentsHandler
    : IRequestHandler<GetAllInstrumentsQuery, Result<GetAllInstrumentsResponse>>
{
    private readonly IInstrumentRepository _instrumentRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetAllInstrumentsHandler(
        IInstrumentRepository instrumentRepository)
    {
        ArgumentNullException.ThrowIfNull(instrumentRepository);
        _instrumentRepository = instrumentRepository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetAllInstrumentsResponse>> Handle(
        GetAllInstrumentsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        PageRequest pageRequest = new()
        {
            Page = request.Page,
            PageSize = request.PageSize
        };

        PagedResult<Instrument> pagedResult =
            await _instrumentRepository.GetPagedAsync(
                pageRequest,
                cancellationToken);

        InstrumentListItemResponse[] items =
            pagedResult.Items
                .Select(instrument =>
                    new InstrumentListItemResponse(
                        instrument.Id.Value.ToString(),
                        instrument.Name.Value,
                        instrument.AssetClass.Value,
                        (int)instrument.Type,
                        instrument.Category.Value,
                        instrument.Isin?.Value,
                        instrument.CreatedOn,
                        instrument.IsActive))
                .ToArray();

        GetAllInstrumentsResponse response = new(
            items,
            pagedResult.Page,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages);

        return Result<GetAllInstrumentsResponse>.Success(response);
    }
}
