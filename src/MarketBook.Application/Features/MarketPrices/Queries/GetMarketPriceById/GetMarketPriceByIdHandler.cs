// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence; using MarketBook.Domain.Common; using MarketBook.Domain.MarketData.Aggregates; using MarketBook.Domain.MarketData.ValueObjects; using MediatR; namespace MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById;
/// <summary>EN: Handles MarketPrice lookup. FA: دریافت MarketPrice را مدیریت می‌کند.</summary>
public sealed class GetMarketPriceByIdHandler:IRequestHandler<GetMarketPriceByIdQuery,Result<GetMarketPriceByIdResponse>>{private readonly IMarketPriceRepository _repository;
/// <summary>EN: Initializes handler. FA: Handler را مقداردهی می‌کند.</summary>
public GetMarketPriceByIdHandler(IMarketPriceRepository repository){ArgumentNullException.ThrowIfNull(repository);_repository=repository;}
/// <summary>EN: Handles query. FA: پرس‌وجو را پردازش می‌کند.</summary>
public async Task<Result<GetMarketPriceByIdResponse>> Handle(GetMarketPriceByIdQuery request,CancellationToken cancellationToken){if(!MarketPriceId.TryParse(request.Id,out MarketPriceId? id)||id is null)return Result<GetMarketPriceByIdResponse>.Fail(new Error("MarketPrice.InvalidId","The identifier is invalid."));MarketPrice? item=await _repository.GetByIdAsync(id,cancellationToken);if(item is null)return Result<GetMarketPriceByIdResponse>.Fail(new Error("MarketPrice.NotFound","The market-price record was not found."));return Result<GetMarketPriceByIdResponse>.Success(new GetMarketPriceByIdResponse(item.Id.Value.ToString(),item.ListingId.Value.ToString(),item.TradingDate,item.OpenPrice,item.HighPrice,item.LowPrice,item.LastPrice,item.ClosePrice,item.PreviousClosePrice,item.ReferencePrice,item.LowerLimit,item.UpperLimit,item.IsAtUpperLimit,item.IsAtLowerLimit,item.CreatedOn,item.UpdatedOn));}}
