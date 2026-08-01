// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Common.Interfaces
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MediatR;

namespace MarketBook.Application.Common.Interfaces.Messaging;

/// <summary>
/// EN: Represents an application command that changes the system state.
/// FA: نمایانگر یک فرمان برنامه است که وضعیت سیستم را تغییر می‌دهد.
/// </summary>
/// <typeparam name="TResult">
/// EN: Command execution result.
/// FA: نتیجه اجرای فرمان.
/// </typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}