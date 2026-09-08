// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : API
// Namespace : MarketBook.Api.Controllers
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Api.Common; using MarketBook.Application.Features.MarketPrices.Commands.CreateMarketPrice; using MarketBook.Application.Features.MarketPrices.Commands.UpdateMarketPrice; using MarketBook.Application.Features.MarketPrices.Queries.GetAllMarketPrices; using MarketBook.Application.Features.MarketPrices.Queries.GetMarketPriceById; using MarketBook.Domain.Common; using MarketBook.Domain.MarketData.ValueObjects; using MediatR; using Microsoft.AspNetCore.Mvc; namespace MarketBook.Api.Controllers;
/// <summary>EN: Provides MarketPrice endpoints. FA: Endpointهای MarketPrice را فراهم می‌کند.</summary>
[ApiController][Route("api/v1/market-prices")] public sealed class MarketPricesController:ControllerBase{private readonly ISender _sender;
/// <summary>EN: Initializes controller. FA: Controller را مقداردهی می‌کند.</summary>
public MarketPricesController(ISender sender){ArgumentNullException.ThrowIfNull(sender);_sender=sender;}
/// <summary>EN: Creates a daily price record. FA: رکورد قیمت روزانه ایجاد می‌کند.</summary>
[HttpPost] public async Task<IActionResult> Create([FromBody]CreateMarketPriceRequest request,CancellationToken cancellationToken){CreateMarketPriceCommand command=new(request.ListingId,request.TradingDate,request.OpenPrice,request.HighPrice,request.LowPrice,request.LastPrice,request.ClosePrice,request.PreviousClosePrice,request.ReferencePrice,request.LowerLimit,request.UpperLimit);Result<MarketPriceId> result=await _sender.Send(command,cancellationToken);return result.IsSuccess?CreatedAtAction(nameof(GetById),new{id=result.Value!.Value.ToString()},new{id=result.Value!.Value.ToString()}):ApiErrorMapper.ToActionResult(this,result.Error);}
/// <summary>EN: Updates prices and limits. FA: قیمت‌ها و دامنه مجاز را به‌روزرسانی می‌کند.</summary>
[HttpPut("{id}")] public async Task<IActionResult> Update(string id,[FromBody]UpdateMarketPriceRequest request,CancellationToken cancellationToken){Result<MarketPriceId> result=await _sender.Send(new UpdateMarketPriceCommand(id,request.OpenPrice,request.HighPrice,request.LowPrice,request.LastPrice,request.ClosePrice,request.PreviousClosePrice,request.ReferencePrice,request.LowerLimit,request.UpperLimit),cancellationToken);return result.IsSuccess?Ok(new{id=result.Value!.Value.ToString()}):ApiErrorMapper.ToActionResult(this,result.Error);}
/// <summary>EN: Gets by id. FA: با شناسه دریافت می‌کند.</summary>
[HttpGet("{id}")] public async Task<IActionResult> GetById(string id,CancellationToken cancellationToken){Result<GetMarketPriceByIdResponse> result=await _sender.Send(new GetMarketPriceByIdQuery(id),cancellationToken);return result.IsSuccess?Ok(result.Value):ApiErrorMapper.ToActionResult(this,result.Error);}
/// <summary>EN: Gets paged records. FA: رکوردهای صفحه‌بندی‌شده را دریافت می‌کند.</summary>
[HttpGet] public async Task<IActionResult> GetAll([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken cancellationToken=default){Result<GetAllMarketPricesResponse> result=await _sender.Send(new GetAllMarketPricesQuery(page,pageSize),cancellationToken);return result.IsSuccess?Ok(result.Value):ApiErrorMapper.ToActionResult(this,result.Error);}}
