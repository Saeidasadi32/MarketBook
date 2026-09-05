// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Domain.Common;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Queries.GetAllCurrencies;
/// <summary>EN: Represents a paged currency query. FA: پرس‌وجوی صفحه‌بندی‌شده ارزها را نشان می‌دهد.</summary>
public sealed record GetAllCurrenciesQuery(int Page=1,int PageSize=20) : IRequest<Result<GetAllCurrenciesResponse>>;
