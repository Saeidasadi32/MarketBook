BEGIN TRANSACTION;
CREATE TABLE [MarketPrices] (
    [Id] varchar(26) NOT NULL,
    [ListingId] varchar(26) NOT NULL,
    [TradingDate] date NOT NULL,
    [OpenPrice] decimal(38,6) NOT NULL,
    [HighPrice] decimal(38,6) NOT NULL,
    [LowPrice] decimal(38,6) NOT NULL,
    [LastPrice] decimal(38,6) NOT NULL,
    [ClosePrice] decimal(38,6) NOT NULL,
    [PreviousClosePrice] decimal(38,6) NOT NULL,
    [ReferencePrice] decimal(38,6) NOT NULL,
    [LowerLimit] decimal(38,6) NOT NULL,
    [UpperLimit] decimal(38,6) NOT NULL,
    [CreatedOn] datetimeoffset NOT NULL,
    [UpdatedOn] datetimeoffset NOT NULL,
    [Version] int NOT NULL,
    CONSTRAINT [PK_MarketPrices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MarketPrices_Listings_ListingId] FOREIGN KEY ([ListingId]) REFERENCES [Listings] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [UX_MarketPrices_Listing_TradingDate] ON [MarketPrices] ([ListingId], [TradingDate]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260906230000_AddMarketPrice', N'10.0.10');

COMMIT;
GO

