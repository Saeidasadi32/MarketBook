// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Countries.Commands.CreateCountry
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Domain.Common;
using MarketBook.Domain.Country.ValueObjects;

namespace MarketBook.Application.Features.Countries.Commands.CreateCountry;

/// <summary>
/// EN: Represents a command for creating a country.
/// FA: فرمان ایجاد یک کشور را نمایش می‌دهد.
/// </summary>
public sealed record CreateCountryCommand(
    string Code,
    string Name,
    string TimeZone)
    : ICommand<Result<CountryId>>;
