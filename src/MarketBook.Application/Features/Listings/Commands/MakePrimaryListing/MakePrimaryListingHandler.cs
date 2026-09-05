// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.MakePrimaryListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.MakePrimaryListing;

/// <summary>
/// EN: Makes a listing the single primary listing for its instrument.
/// FA: یک Listing را به تنها پذیرش اصلی Instrument تبدیل می‌کند.
/// </summary>
public sealed class MakePrimaryListingHandler
    : IRequestHandler<MakePrimaryListingCommand, Result<ListingId>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the handler.
    /// FA: Handler را مقداردهی می‌کند.
    /// </summary>
    public MakePrimaryListingHandler(
        IListingRepository listingRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(listingRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _listingRepository = listingRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the command while preserving one primary listing per instrument.
    /// FA: فرمان را با حفظ قاعده یک Listing اصلی برای هر Instrument پردازش می‌کند.
    /// </summary>
    public async Task<Result<ListingId>> Handle(
        MakePrimaryListingCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ListingId.TryParse(request.Id, out ListingId? listingId) ||
            listingId is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidId", "The specified listing identifier is invalid."));
        }

        Listing? listing =
            await _listingRepository.GetByIdAsync(listingId, cancellationToken);

        if (listing is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.NotFound", "The specified listing was not found."));
        }

        if (listing.IsPrimary)
        {
            return Result<ListingId>.Success(listing.Id);
        }

        Listing? currentPrimary =
            await _listingRepository.GetPrimaryByInstrumentIdAsync(
                listing.InstrumentId,
                cancellationToken);

        if (currentPrimary is not null &&
            currentPrimary.Id != listing.Id)
        {
            currentPrimary.RemovePrimary();
            _listingRepository.Update(currentPrimary);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        listing.MakePrimary();
        _listingRepository.Update(listing);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<ListingId>.Success(listing.Id);
    }
}
