// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Abstractions.Messaging
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MediatR;

namespace MarketBook.Application.Abstractions.Messaging;

/// <summary>
/// EN: Defines a handler for a query.
/// FA: پردازش‌کننده یک پرس‌وجو را تعریف می‌کند.
/// </summary>
/// <typeparam name="TQuery">
/// EN: Query type.
/// FA: نوع پرس‌وجو.
/// </typeparam>
/// <typeparam name="TResponse">
/// EN: Query response.
/// FA: پاسخ پرس‌وجو.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}