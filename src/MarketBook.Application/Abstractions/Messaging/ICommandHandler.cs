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
/// EN: Defines a handler for an application command.
/// FA: پردازش‌کننده یک فرمان برنامه را تعریف می‌کند.
/// </summary>
/// <typeparam name="TCommand">
/// EN: Command type.
/// FA: نوع فرمان.
/// </typeparam>
/// <typeparam name="TResponse">
/// EN: Command response.
/// FA: پاسخ فرمان.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> :
    IRequestHandler<TCommand, TResponse>
    where TCommand : notnull, ICommand<TResponse>
{
}