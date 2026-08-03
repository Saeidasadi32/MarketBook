// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using FluentValidation;
using MarketBook.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MarketBook.Application;

/// <summary>
/// EN: Provides dependency injection registration methods for the Application layer.
/// FA: متدهای ثبت وابستگی‌های لایه Application را فراهم می‌کند.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// EN: Registers all Application services.
    /// FA: تمام سرویس‌های لایه Application را ثبت می‌کند.
    /// </summary>
    /// <param name="services">
    /// EN: Service collection.
    /// FA: مجموعه سرویس‌ها.
    /// </param>
    /// <returns>
    /// EN: Updated service collection.
    /// FA: مجموعه سرویس‌های به‌روزشده.
    /// </returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(UnhandledExceptionBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(PerformanceBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}
