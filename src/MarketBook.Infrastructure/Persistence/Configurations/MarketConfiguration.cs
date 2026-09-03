using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketBook.Infrastructure.Persistence.Configurations;

/// <summary>
/// EN: Configures the Market aggregate for Entity Framework Core persistence.
/// FA: تنظیمات Aggregate بازار را برای ماندگاری توسط Entity Framework Core تعریف می‌کند.
/// </summary>
public sealed class MarketConfiguration : IEntityTypeConfiguration<Market>
{
    /// <summary>
    /// EN: Configures the Market entity, value objects, relationships, indexes, and constraints.
    /// FA: موجودیت بازار، Value Objectها، روابط، ایندکس‌ها و محدودیت‌های بازار را پیکربندی می‌کند.
    /// </summary>
    /// <param name="builder">
    /// EN: Entity type builder for the Market aggregate.
    /// FA: سازنده تنظیمات موجودیت Aggregate بازار.
    /// </param>
    public void Configure(EntityTypeBuilder<Market> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Markets");

        builder.HasKey(market => market.Id);

        builder.Property(market => market.Id)
            .HasConversion(
                marketId => marketId.Value.ToString(),
                value => MarketId.Parse(value))
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired();

        ValueConverter<MarketCode, string> marketCodeConverter =
            new(
                code => code.Value,
                value => new MarketCode(value));

        builder.Property(market => market.Code)
            .HasConversion(marketCodeConverter)
            .HasMaxLength(32)
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(market => market.Code)
            .IsUnique();

        builder.Property(market => market.Name)
            .HasMaxLength(200)
            .IsUnicode()
            .IsRequired();

        ValueConverter<ExchangeId?, string?> exchangeIdConverter =
            new(
                exchangeId => exchangeId == null
                    ? null
                    : exchangeId.Value.ToString(),
                value => value == null
                    ? null
                    : ExchangeId.Parse(value));

        builder.Property(market => market.ExchangeId)
            .HasConversion(exchangeIdConverter)
            .HasMaxLength(26)
            .IsUnicode(false)
            .IsRequired(false);

        builder.HasOne<Exchange>()
            .WithMany()
            .HasForeignKey(market => market.ExchangeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(market => market.CreatedOn)
            .IsRequired();

        builder.Property(market => market.IsActive)
            .IsRequired();
    }
}
