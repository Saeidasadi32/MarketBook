// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
namespace MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies;
/// <summary>EN: Represents a paged currency response. FA: پاسخ صفحه‌بندی‌شده ارزها را نشان می‌دهد.</summary>
public sealed record GetAllCurrenciesResponse(IReadOnlyCollection<CurrencyListItemResponse> Items,int Page,int PageSize,int TotalCount,int TotalPages);
/// <summary>EN: Represents a currency list item. FA: یک آیتم از فهرست ارزها را نشان می‌دهد.</summary>
public sealed record CurrencyListItemResponse(string Id,string Code,string Name,int DecimalPlaces,DateTimeOffset CreatedOn,bool IsActive);
