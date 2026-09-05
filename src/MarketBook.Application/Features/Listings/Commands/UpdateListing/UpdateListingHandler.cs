// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Listings.Commands.UpdateListing
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MarketBook.Domain.Listing.Aggregates;
using MarketBook.Domain.Listing.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Listings.Commands.UpdateListing;

/// <summary>
/// EN: Handles updating mutable Listing attributes.
/// FA: به‌روزرسانی ویژگی‌های قابل تغییر Listing را مدیریت می‌کند.
/// </summary>
public sealed class UpdateListingHandler
    : IRequestHandler<UpdateListingCommand, Result<ListingId>>
{
    private readonly IListingRepository _listingRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes the update listing handler.
    /// FA: Handler به‌روزرسانی Listing را مقداردهی می‌کند.
    /// </summary>
    public UpdateListingHandler(
        IListingRepository listingRepository,
        ICurrencyRepository currencyRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(listingRepository);
        ArgumentNullException.ThrowIfNull(currencyRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _listingRepository = listingRepository;
        _currencyRepository = currencyRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the update listing command.
    /// FA: فرمان به‌روزرسانی Listing را پردازش می‌کند.
    /// </summary>
    public async Task<Result<ListingId>> Handle(
        UpdateListingCommand request,
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
                listing.VenueId,
                tradingSymbol,
                listing.Id,
                cancellationToken))
        {
            return Result<ListingId>.Fail(
                new Error(
                    "Listing.DuplicateTradingSymbol",
                    "The trading symbol already exists in the specified venue."));
        }

        listing.ChangeQuoteCurrency(currencyId);
        listing.ChangeTradingSymbol(tradingSymbol);
        listing.ChangePricing(request.TickSize, (byte)request.PricePrecision);

        _listingRepository.Update(listing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<ListingId>.Success(listing.Id);
    }
}
