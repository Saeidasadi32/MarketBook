// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Configurations
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Listing.Aggregates; using MarketBook.Domain.Listing.ValueObjects; using MarketBook.Domain.MarketData.Aggregates; using MarketBook.Domain.MarketData.ValueObjects; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Metadata.Builders; using Microsoft.EntityFrameworkCore.Storage.ValueConversion; namespace MarketBook.Infrastructure.Persistence.Configurations;
/// <summary>EN: Configures MarketPrice persistence. FA: نگاشت ماندگاری MarketPrice را پیکربندی می‌کند.</summary>
public sealed class MarketPriceConfiguration:IEntityTypeConfiguration<MarketPrice>{
/// <summary>EN: Configures mapping. FA: نگاشت را پیکربندی می‌کند.</summary>
public void Configure(EntityTypeBuilder<MarketPrice> builder){ArgumentNullException.ThrowIfNull(builder);builder.ToTable("MarketPrices");builder.HasKey(item=>item.Id);ValueConverter<MarketPriceId,string> idConverter=new(id=>id.Value.ToString(),value=>MarketPriceId.Parse(value));builder.Property(item=>item.Id).HasConversion(idConverter).HasMaxLength(26).IsUnicode(false).IsRequired();ValueConverter<ListingId,string> listingConverter=new(id=>id.Value.ToString(),value=>ListingId.Parse(value));builder.Property(item=>item.ListingId).HasConversion(listingConverter).HasMaxLength(26).IsUnicode(false).IsRequired();builder.Property(item=>item.TradingDate).HasColumnType("date").IsRequired();builder.Property(item=>item.OpenPrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.HighPrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.LowPrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.LastPrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.ClosePrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.PreviousClosePrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.ReferencePrice).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.LowerLimit).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.UpperLimit).HasColumnType("decimal(38,6)").IsRequired();builder.Property(item=>item.CreatedOn).IsRequired();builder.Property(item=>item.UpdatedOn).IsRequired();builder.HasIndex(item=>new{item.ListingId,item.TradingDate}).IsUnique().HasDatabaseName("UX_MarketPrices_Listing_TradingDate");builder.HasOne<Listing>().WithMany().HasForeignKey(item=>item.ListingId).OnDelete(DeleteBehavior.Restrict).IsRequired();}}
