// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Queries.GetCurrencyById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
namespace MarketBook.Application.Features.Currencies.Queries.GetCurrencyById;
/// <summary>EN: Represents a currency response. FA: پاسخ ارز را نشان می‌دهد.</summary>
public sealed record GetCurrencyByIdResponse(string Id,string Code,string Name,int DecimalPlaces,DateTimeOffset CreatedOn,bool IsActive);
