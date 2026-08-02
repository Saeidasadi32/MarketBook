// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Country.Commands.CreateCountry
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Messaging;
using MarketBook.Domain.Common;

namespace MarketBook.Application.Features.Country.Commands.CreateCountry;

/// <summary>
/// EN: Represents a command for creating a country.
/// FA: فرمان ایجاد یک کشور را نمایش می‌دهد.
/// </summary>
/// <param name="Code">
/// EN: ISO country code.
/// FA: کد استاندارد ISO کشور.
/// </param>
/// <param name="Name">
/// EN: Country name.
/// FA: نام کشور.
/// </param>
public sealed record CreateCountryCommand(
    string Code,
    string Name)
    : ICommand<Result>;