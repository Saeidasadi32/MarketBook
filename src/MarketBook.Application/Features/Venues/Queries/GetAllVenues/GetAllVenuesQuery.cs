// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Venues.Queries.GetAllVenues
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using MediatR;

namespace MarketBook.Application.Features.Venues.Queries.GetAllVenues;

/// <summary>
/// EN: Represents the query used to retrieve a paged collection of trading venues.
/// FA: پرس‌وجوی دریافت مجموعه‌ای صفحه‌بندی‌شده از بسترهای معاملاتی را نمایش می‌دهد.
/// </summary>
/// <param name="Page">
/// EN: Requested page number.
/// FA: شماره صفحه مورد درخواست.
/// </param>
/// <param name="PageSize">
/// EN: Requested number of items per page.
/// FA: تعداد موارد مورد درخواست در هر صفحه.
/// </param>
public sealed record GetAllVenuesQuery(
    int Page = 1,
    int PageSize = 20)
    : IRequest<Result<GetAllVenuesResponse>>;
