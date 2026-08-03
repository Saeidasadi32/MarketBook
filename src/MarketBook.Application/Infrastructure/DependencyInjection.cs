// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketBook.Infrastructure;

/// <summary>
/// EN: Provides dependency injection registration methods for the Infrastructure layer.
/// FA: متدهای ثبت وابستگی‌های لایه Infrastructure را فراهم می‌کند.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// EN: Registers all Infrastructure services.
    /// FA: تمام سرویس‌های لایه Infrastructure را ثبت می‌کند.
    /// </summary>
    /// <param name="services">
    /// EN: Service collection.
    /// FA: مجموعه سرویس‌ها.
    /// </param>
    /// <param name="configuration">
    /// EN: Application configuration.
    /// FA: تنظیمات برنامه.
    /// </param>
    /// <returns>
    /// EN: Updated service collection.
    /// FA: مجموعه سرویس‌های به‌روزشده.
    /// </returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
