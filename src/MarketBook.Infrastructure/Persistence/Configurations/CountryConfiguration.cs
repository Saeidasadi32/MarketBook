// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common.ValueObjects;
using MarketBook.Domain.Country.Aggregates;
using MarketBook.Domain.Country.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures the Country aggregate for Entity Framework Core.
/// FA: Aggregate کشور را برای Entity Framework Core پیکربندی می‌کند.
/// </summary>
internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ValueConverter<CountryId, string> idConverter =
            new(
                id => id.Value.ToString(),
                value => CountryId.Parse(value));

        ValueConverter<CountryCode, string> codeConverter =
            new(
                code => code.Value,
                value => new CountryCode(value));

        ValueConverter<TimeZoneId, string> timeZoneConverter =
            new(
                value => value.Value,
                value => new TimeZoneId(value));

        builder.ToTable("Countries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(idConverter)
            .HasMaxLength(26)
            .ValueGeneratedNever();

        builder.Property(x => x.Code)
            .HasConversion(codeConverter)
            .HasMaxLength(2)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TimeZone)
            .HasConversion(timeZoneConverter)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsConcurrencyToken()
            .IsRequired();
    }
}
