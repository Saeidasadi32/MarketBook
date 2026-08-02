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
/// EN: Represents an application query.
/// FA: نمایانگر یک پرس‌وجوی برنامه است.
/// </summary>
/// <typeparam name="TResponse">
/// EN: Query response.
/// FA: پاسخ پرس‌وجو.
/// </typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}