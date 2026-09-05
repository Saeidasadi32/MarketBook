// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Queries.GetCurrencyById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Currency.Aggregates;
using MarketBook.Domain.Currency.ValueObjects;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Queries.GetCurrencyById;
/// <summary>EN: Handles retrieving a currency by identifier. FA: دریافت ارز بر اساس شناسه را مدیریت می‌کند.</summary>
public sealed class GetCurrencyByIdHandler : IRequestHandler<GetCurrencyByIdQuery,Result<GetCurrencyByIdResponse>>
{
 private readonly ICurrencyRepository _currencyRepository;
 /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
 public GetCurrencyByIdHandler(ICurrencyRepository currencyRepository){ArgumentNullException.ThrowIfNull(currencyRepository);_currencyRepository=currencyRepository;}
 /// <summary>EN: Handles the query. FA: پرس‌وجو را پردازش می‌کند.</summary>
 public async Task<Result<GetCurrencyByIdResponse>> Handle(GetCurrencyByIdQuery request,CancellationToken cancellationToken){ArgumentNullException.ThrowIfNull(request);if(!CurrencyId.TryParse(request.Id,out CurrencyId? id)||id is null)return Result<GetCurrencyByIdResponse>.Fail(new Error("Currency.InvalidId","The specified currency identifier is invalid."));Currency? c=await _currencyRepository.GetByIdAsync(id,cancellationToken);if(c is null)return Result<GetCurrencyByIdResponse>.Fail(new Error("Currency.NotFound","The specified currency was not found."));return Result<GetCurrencyByIdResponse>.Success(new(c.Id.Value.ToString(),c.Code.Value,c.Name,c.DecimalPlaces,c.CreatedOn,c.IsActive));}
}
