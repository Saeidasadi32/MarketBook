// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence; using MarketBook.Application.Common.Pagination; using MarketBook.Domain.Common; using MarketBook.Domain.MarketData.Aggregates; using MediatR; namespace MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices;
/// <summary>EN: Handles paged MarketPrice query. FA: پرس‌وجوی صفحه‌بندی MarketPrice را مدیریت می‌کند.</summary>
public sealed class GetAllMarketPricesHandler:IRequestHandler<GetAllMarketPricesQuery,Result<GetAllMarketPricesResponse>>{private readonly IMarketPriceRepository _repository;
/// <summary>EN: Initializes handler. FA: Handler را مقداردهی می‌کند.</summary>
public GetAllMarketPricesHandler(IMarketPriceRepository repository){ArgumentNullException.ThrowIfNull(repository);_repository=repository;}
/// <summary>EN: Handles query. FA: پرس‌وجو را پردازش می‌کند.</summary>
public async Task<Result<GetAllMarketPricesResponse>> Handle(GetAllMarketPricesQuery request,CancellationToken cancellationToken){PageRequest pageRequest=new(){Page=request.Page,PageSize=request.PageSize};PagedResult<MarketPrice> page=await _repository.GetPagedAsync(pageRequest,cancellationToken);MarketPriceListItemResponse[] items=page.Items.Select(item=>new MarketPriceListItemResponse(item.Id.Value.ToString(),item.ListingId.Value.ToString(),item.TradingDate,item.LastPrice,item.ClosePrice,item.LowerLimit,item.UpperLimit)).ToArray();return Result<GetAllMarketPricesResponse>.Success(new GetAllMarketPricesResponse(items,page.Page,page.PageSize,page.TotalCount,page.TotalPages));}}
