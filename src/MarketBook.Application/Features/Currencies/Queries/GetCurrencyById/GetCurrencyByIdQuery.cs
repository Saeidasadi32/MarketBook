// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Currencies.Queries.GetCurrencyById
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------
using MarketBook.Domain.Common;
using MediatR;
namespace MarketBook.Application.Features.Currencies.Queries.GetCurrencyById;
/// <summary>EN: Represents a query to get a currency by identifier. FA: پرس‌وجوی دریافت ارز بر اساس شناسه را نشان می‌دهد.</summary>
public sealed record GetCurrencyByIdQuery(string Id) : IRequest<Result<GetCurrencyByIdResponse>>;
