// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.Mappings
// -----------------------------------------------------------------------------

using MarketBook.Application.Features.Countries.Responses;
using MarketBook.Domain.Country.Aggregates;

namespace MarketBook.Application.Features.Countries.Mappings;

/// <summary>
/// EN: Country mapping extensions.
/// FA: متدهای تبدیل Country.
/// </summary>
public static class CountryMappings
{
    /// <summary>
    /// EN: Converts entity to response.
    /// FA: تبدیل موجودیت به پاسخ.
    /// </summary>
    public static CountryResponse ToResponse(
        this Country country)
    {
        ArgumentNullException.ThrowIfNull(country);

        return new CountryResponse(
            country.Id.ToString(),
            country.Code.Value,
            country.Name,
            country.TimeZone.Value,
            country.IsActive);
    }
}
