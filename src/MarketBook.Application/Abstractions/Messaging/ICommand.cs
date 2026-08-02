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
/// EN: Represents an application command that changes the system state.
/// FA: نمایانگر یک فرمان برنامه است که وضعیت سیستم را تغییر می‌دهد.
/// </summary>
/// <typeparam name="TResponse">
/// EN: Command response.
/// FA: پاسخ فرمان.
/// </typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}