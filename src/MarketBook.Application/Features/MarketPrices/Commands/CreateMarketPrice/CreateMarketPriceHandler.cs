// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence; using MarketBook.Domain.Common; using MarketBook.Domain.Listing.Aggregates; using MarketBook.Domain.Listing.ValueObjects; using MarketBook.Domain.MarketData.Aggregates; using MarketBook.Domain.MarketData.ValueObjects; using MediatR;
namespace MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice;
/// <summary>EN: Handles MarketPrice creation. FA: ایجاد MarketPrice را مدیریت می‌کند.</summary>
public sealed class CreateMarketPriceHandler : IRequestHandler<CreateMarketPriceCommand,Result<MarketPriceId>>
{
 private readonly IMarketPriceRepository _repository; private readonly IListingRepository _listingRepository; private readonly IApplicationDbContext _dbContext;
/// <summary>EN: Initializes handler. FA: Handler را مقداردهی می‌کند.</summary>
public CreateMarketPriceHandler(IMarketPriceRepository repository,IListingRepository listingRepository,IApplicationDbContext dbContext){ArgumentNullException.ThrowIfNull(repository);ArgumentNullException.ThrowIfNull(listingRepository);ArgumentNullException.ThrowIfNull(dbContext);_repository=repository;_listingRepository=listingRepository;_dbContext=dbContext;}
/// <summary>EN: Handles command. FA: فرمان را پردازش می‌کند.</summary>
public async Task<Result<MarketPriceId>> Handle(CreateMarketPriceCommand request,CancellationToken cancellationToken)
 { ArgumentNullException.ThrowIfNull(request); if(!ListingId.TryParse(request.ListingId,out ListingId? listingId)||listingId is null) return Result<MarketPriceId>.Fail(new Error("MarketPrice.InvalidListingId","The listing identifier is invalid.")); Listing? listing=await _listingRepository.GetByIdAsync(listingId,cancellationToken); if(listing is null) return Result<MarketPriceId>.Fail(new Error("MarketPrice.ListingNotFound","The listing was not found.")); if(!listing.IsActive) return Result<MarketPriceId>.Fail(new Error("MarketPrice.ListingInactive","The listing is inactive.")); if(await _repository.ExistsAsync(listingId,request.TradingDate,cancellationToken)) return Result<MarketPriceId>.Fail(new Error("MarketPrice.DuplicateListingDate","A market-price record already exists for this Listing and trading date.")); MarketPrice marketPrice; try { marketPrice=MarketPrice.Create(listingId,request.TradingDate,request.OpenPrice,request.HighPrice,request.LowPrice,request.LastPrice,request.ClosePrice,request.PreviousClosePrice,request.ReferencePrice,request.LowerLimit,request.UpperLimit); } catch(ArgumentException exception) { return Result<MarketPriceId>.Fail(new Error("MarketPrice.InvalidPrices",exception.Message)); } await _repository.AddAsync(marketPrice,cancellationToken); await _dbContext.SaveChangesAsync(cancellationToken); return Result<MarketPriceId>.Success(marketPrice.Id); }
}
