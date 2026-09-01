// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Application
// Namespace : MarketBook.Application.Features.Exchanges.Mappings
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Exchanges.Responses;
using MarketBook.Domain.Exchange.Aggregates;

namespace MarketBook.Application.Features.Exchanges.Mappings;

/// <summary>
/// EN: Exchange mapping extensions.
/// FA: متدهای تبدیل Exchange.
/// </summary>
public static class ExchangeMappings
{
    /// <summary>
    /// EN: Converts an Exchange entity to an ExchangeResponse.
    /// FA: موجودیت Exchange را به ExchangeResponse تبدیل می‌کند.
    /// </summary>
    public static ExchangeResponse ToResponse(
        this Exchange exchange)
    {
        ArgumentNullException.ThrowIfNull(exchange);

        return new ExchangeResponse(
            exchange.Id.Value.ToString(),
            exchange.CountryId?.Value.ToString(),
            exchange.Code.Value,
            exchange.Name,
            exchange.CreatedOn,
            exchange.IsActive);
    }
}
