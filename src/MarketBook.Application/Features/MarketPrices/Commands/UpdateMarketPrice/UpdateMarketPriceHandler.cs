// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Commands.UpdateMarketPrice
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence; using MarketBook.Domain.Common; using MarketBook.Domain.MarketData.Aggregates; using MarketBook.Domain.MarketData.ValueObjects; using MediatR; namespace MarketBook.Application.Features.MarketPrices.Commands.UpdateMarketPrice;
/// <summary>EN: Handles MarketPrice update. FA: به‌روزرسانی MarketPrice را مدیریت می‌کند.</summary>
public sealed class UpdateMarketPriceHandler:IRequestHandler<UpdateMarketPriceCommand,Result<MarketPriceId>> { private readonly IMarketPriceRepository _repository; private readonly IApplicationDbContext _dbContext;
/// <summary>EN: Initializes handler. FA: Handler را مقداردهی می‌کند.</summary>
public UpdateMarketPriceHandler(IMarketPriceRepository repository,IApplicationDbContext dbContext){ArgumentNullException.ThrowIfNull(repository);ArgumentNullException.ThrowIfNull(dbContext);_repository=repository;_dbContext=dbContext;}
/// <summary>EN: Handles command. FA: فرمان را پردازش می‌کند.</summary>
public async Task<Result<MarketPriceId>> Handle(UpdateMarketPriceCommand request,CancellationToken cancellationToken){ if(!MarketPriceId.TryParse(request.Id,out MarketPriceId? id)||id is null)return Result<MarketPriceId>.Fail(new Error("MarketPrice.InvalidId","The identifier is invalid.")); MarketPrice? item=await _repository.GetByIdAsync(id,cancellationToken); if(item is null)return Result<MarketPriceId>.Fail(new Error("MarketPrice.NotFound","The market-price record was not found.")); try{item.Update(request.OpenPrice,request.HighPrice,request.LowPrice,request.LastPrice,request.ClosePrice,request.PreviousClosePrice,request.ReferencePrice,request.LowerLimit,request.UpperLimit);}catch(ArgumentException exception){return Result<MarketPriceId>.Fail(new Error("MarketPrice.InvalidPrices",exception.Message));}_repository.Update(item);await _dbContext.SaveChangesAsync(cancellationToken);return Result<MarketPriceId>.Success(item.Id);}}
