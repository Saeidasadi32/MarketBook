// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.MarketPrices
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

namespace MarketBook.Application.Features.MarketPrices;
/// <summary>EN: Carries writable market-price fields. FA: فیلدهای قابل ثبت قیمت بازار را حمل می‌کند.</summary>
public sealed record MarketPriceValues(decimal OpenPrice,decimal HighPrice,decimal LowPrice,decimal LastPrice,decimal ClosePrice,decimal PreviousClosePrice,decimal ReferencePrice,decimal LowerLimit,decimal UpperLimit);
