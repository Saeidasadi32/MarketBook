// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Queries.GetListingById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Queries.GetListingById;

/// <summary>
/// EN: Handles retrieving a listing by identifier.
/// FA: دریافت Listing بر اساس شناسه را مدیریت می‌کند.
/// </summary>
public sealed class GetListingByIdHandler
    : IRequestHandler<GetListingByIdQuery, Result<GetListingByIdResponse>>
{
    private readonly IListingRepository _listingRepository;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public GetListingByIdHandler(IListingRepository listingRepository)
    {
        ArgumentNullException.ThrowIfNull(listingRepository);
        _listingRepository = listingRepository;
    }

    /// <summary>
    /// EN: Handles the query.
    /// FA: پرس‌وجو را پردازش می‌کند.
    /// </summary>
    public async Task<Result<GetListingByIdResponse>> Handle(
        GetListingByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.Id, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<GetListingByIdResponse>.Fail(
                new Error("Listing.InvalidId", "The specified listing identifier is invalid."));
        }

        Listing? listing =
            await _listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
        {
            return Result<GetListingByIdResponse>.Fail(
                new Error("Listing.NotFound", "The specified listing was not found."));
        }

        GetListingByIdResponse response = new(
            listing.Id.Value.ToString(),
            listing.InstrumentId.Value.ToString(),
            listing.VenueId.Value.ToString(),
            listing.QuoteCurrencyId.Value.ToString(),
            listing.TradingSymbol.Value,
            listing.TickSize,
            listing.PricePrecision,
            listing.IsPrimary,
            listing.CreatedOn,
            listing.IsActive,
            listing.ActivatedOn,
            listing.DeactivatedOn);

        return Result<GetListingByIdResponse>.Success(response);
    }
}
