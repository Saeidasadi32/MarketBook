// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Queries.GetVenueById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Venues.Queries.GetVenueById;

/// <summary>
/// EN: Handles the query used to retrieve a trading venue by its identifier.
/// FA: پرس‌وجوی دریافت یک بستر معاملاتی بر اساس شناسه آن را پردازش می‌کند.
/// </summary>
public sealed class GetVenueByIdHandler
    : IRequestHandler<GetVenueByIdQuery, Result<GetVenueByIdResponse>>
{
    private readonly IVenueRepository _venueRepository;

    /// <summary>
    /// EN: Initializes a new instance of the <see cref="GetVenueByIdHandler"/> class.
    /// FA: یک نمونه جدید از کلاس <see cref="GetVenueByIdHandler"/> ایجاد می‌کند.
    /// </summary>
    /// <param name="venueRepository">
    /// EN: Trading venue repository.
    /// FA: مخزن بسترهای معاملاتی.
    /// </param>
    public GetVenueByIdHandler(
        IVenueRepository venueRepository)
    {
        ArgumentNullException.ThrowIfNull(venueRepository);

        _venueRepository = venueRepository;
    }

    /// <summary>
    /// EN: Retrieves a trading venue by its identifier.
    /// FA: یک بستر معاملاتی را بر اساس شناسه آن دریافت می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Venue retrieval query.
    /// FA: پرس‌وجوی دریافت بستر معاملاتی.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The venue response or an application error.
    /// FA: پاسخ بستر معاملاتی یا خطای برنامه.
    /// </returns>
    public async Task<Result<GetVenueByIdResponse>> Handle(
        GetVenueByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!VenueId.TryParse(
                request.Id,
                out VenueId? venueId) ||
            venueId is null)
        {
            return Result<GetVenueByIdResponse>.Fail(
                new Error(
                    "Venue.InvalidId",
                    "The specified venue identifier is invalid."));
        }

        Venue? venue =
            await _venueRepository.GetByIdAsync(
                venueId,
                cancellationToken);

        if (venue is null)
        {
            return Result<GetVenueByIdResponse>.Fail(
                new Error(
                    "Venue.NotFound",
                    "The specified venue was not found."));
        }

        GetVenueByIdResponse response =
            new(
                venue.Id.Value.ToString(),
                venue.MarketId.Value.ToString(),
                venue.Code.Value,
                venue.Name,
                (int)venue.Type,
                venue.CreatedOn,
                venue.IsActive);

        return Result<GetVenueByIdResponse>.Success(
            response);
    }
}
