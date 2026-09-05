// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.CreateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Instrument.Aggregates;
using MarketBook.Domain.Instrument.ValueObjects;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MarketBook.Domain.Venue.Aggregates;
using MarketBook.Domain.Venue.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.CreateListing;

/// <summary>
/// EN: Handles creating a Listing aggregate.
/// FA: ایجاد Aggregate مربوط به Listing را مدیریت می‌کند.
/// </summary>
public sealed class CreateListingHandler
    : IRequestHandler<CreateListingCommand, Result<ListingId>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IInstrumentRepository _instrumentRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the create listing handler.
    /// FA: Handler ایجاد Listing را مقداردهی می‌کند.
    /// </summary>
    public CreateListingHandler(
        IListingRepository listingRepository,
        IInstrumentRepository instrumentRepository,
        IVenueRepository venueRepository,
        ICurrencyRepository currencyRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(listingRepository);
        ArgumentNullException.ThrowIfNull(instrumentRepository);
        ArgumentNullException.ThrowIfNull(venueRepository);
        ArgumentNullException.ThrowIfNull(currencyRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _listingRepository = listingRepository;
        _instrumentRepository = instrumentRepository;
        _venueRepository = venueRepository;
        _currencyRepository = currencyRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the create listing command.
    /// FA: فرمان ایجاد Listing را پردازش می‌کند.
    /// </summary>
    public async Task<Result<ListingId>> Handle(
        CreateListingCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!InstrumentId.TryParse(request.InstrumentId, out InstrumentId? instrumentId) ||
            instrumentId is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidInstrumentId", "The specified instrument identifier is invalid."));
        }

        Instrument? instrument =
            await _instrumentRepository.GetByIdAsync(instrumentId, cancellationToken);

        if (instrument is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InstrumentNotFound", "The specified instrument was not found."));
        }

        if (!instrument.IsActive)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InstrumentInactive", "The specified instrument is inactive."));
        }

        if (!VenueId.TryParse(request.VenueId, out VenueId? venueId) ||
            venueId is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidVenueId", "The specified venue identifier is invalid."));
        }

        Venue? venue =
            await _venueRepository.GetByIdAsync(venueId, cancellationToken);

        if (venue is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.VenueNotFound", "The specified venue was not found."));
        }

        if (!venue.IsActive)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.VenueInactive", "The specified venue is inactive."));
        }

        if (!CurrencyId.TryParse(request.QuoteCurrencyId, out CurrencyId? currencyId) ||
            currencyId is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidQuoteCurrencyId", "The specified quote currency identifier is invalid."));
        }

        Currency? currency =
            await _currencyRepository.GetByIdAsync(currencyId, cancellationToken);

        if (currency is null)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.QuoteCurrencyNotFound", "The specified quote currency was not found."));
        }

        if (!currency.IsActive)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.QuoteCurrencyInactive", "The specified quote currency is inactive."));
        }

        TradingSymbol tradingSymbol;

        try
        {
            tradingSymbol = new TradingSymbol(request.TradingSymbol);
        }
        catch (ArgumentException)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidTradingSymbol", "The specified trading symbol is invalid."));
        }

        if (request.TickSize <= 0)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidTickSize", "Tick size must be greater than zero."));
        }

        if (request.PricePrecision < 0 || request.PricePrecision > 28)
        {
            return Result<ListingId>.Fail(
                new Error("Listing.InvalidPricePrecision", "Price precision must be between 0 and 28."));
        }

        if (await _listingRepository.ExistsAsync(
                venueId,
                tradingSymbol,
                cancellationToken))
        {
            return Result<ListingId>.Fail(
                new Error(
                    "Listing.DuplicateTradingSymbol",
                    "The trading symbol already exists in the specified venue."));
        }

        Listing listing = Listing.Create(
            instrumentId,
            venueId,
            currencyId,
            tradingSymbol,
            request.TickSize,
            (byte)request.PricePrecision);

        await _listingRepository.AddAsync(listing, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<ListingId>.Success(listing.Id);
    }
}
