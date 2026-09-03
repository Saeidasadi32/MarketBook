using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Common;
using MarketBook.Domain.Exchange.Aggregates;
using MarketBook.Domain.Exchange.ValueObjects;
using MarketBook.Domain.Market.Aggregates;
using MarketBook.Domain.Market.ValueObjects;
using MediatR;

namespace MarketBook.Application.Features.Markets.Commands.CreateMarket;

/// <summary>
/// EN: Handles the creation of a new Market aggregate.
/// FA: ایجاد یک Aggregate جدید از نوع بازار را مدیریت می‌کند.
/// </summary>
public sealed class CreateMarketCommandHandler
    : IRequestHandler<CreateMarketCommand, Result<MarketId>>
{
    private readonly IMarketRepository _marketRepository;
    private readonly IExchangeRepository _exchangeRepository;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>
    /// EN: Initializes a new instance of the Create Market command handler.
    /// FA: یک نمونه جدید از Handler فرمان ایجاد بازار را ایجاد می‌کند.
    /// </summary>
    /// <param name="marketRepository">
    /// EN: Market repository.
    /// FA: Repository بازار.
    /// </param>
    /// <param name="exchangeRepository">
    /// EN: Exchange repository.
    /// FA: Repository بورس.
    /// </param>
    /// <param name="dbContext">
    /// EN: Application database context.
    /// FA: Context پایگاه داده برنامه.
    /// </param>
    public CreateMarketCommandHandler(
        IMarketRepository marketRepository,
        IExchangeRepository exchangeRepository,
        IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(marketRepository);
        ArgumentNullException.ThrowIfNull(exchangeRepository);
        ArgumentNullException.ThrowIfNull(dbContext);

        _marketRepository = marketRepository;
        _exchangeRepository = exchangeRepository;
        _dbContext = dbContext;
    }

    /// <summary>
    /// EN: Handles the request to create a new market.
    /// FA: درخواست ایجاد یک بازار جدید را پردازش می‌کند.
    /// </summary>
    /// <param name="request">
    /// EN: Create Market command.
    /// FA: فرمان ایجاد بازار.
    /// </param>
    /// <param name="cancellationToken">
    /// EN: Cancellation token.
    /// FA: توکن لغو عملیات.
    /// </param>
    /// <returns>
    /// EN: The identifier of the created market when successful; otherwise a domain error.
    /// FA: شناسه بازار ایجادشده در صورت موفقیت؛ در غیر این صورت خطای دامنه.
    /// </returns>
    public async Task<Result<MarketId>> Handle(
        CreateMarketCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        MarketCode code;

        try
        {
            code = new MarketCode(request.Code);
        }
        catch (ArgumentException)
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.InvalidCode",
                    "The specified market code is invalid."));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.InvalidName",
                    "Market name is required."));
        }

        if (await _marketRepository.ExistsAsync(
                code,
                cancellationToken))
        {
            return Result<MarketId>.Fail(
                new Error(
                    "Market.DuplicateCode",
                    "A market with the specified code already exists."));
        }

        ExchangeId? exchangeId = null;

        if (!string.IsNullOrWhiteSpace(request.ExchangeId))
        {
            if (!ExchangeId.TryParse(
                    request.ExchangeId,
                    out ExchangeId? parsedExchangeId) ||
                parsedExchangeId is null)
            {
                return Result<MarketId>.Fail(
                    new Error(
                        "Market.InvalidExchangeId",
                        "The specified exchange identifier is invalid."));
            }

            Exchange? exchange =
                await _exchangeRepository.GetByIdAsync(
                    parsedExchangeId,
                    cancellationToken);

            if (exchange is null)
            {
                return Result<MarketId>.Fail(
                    new Error(
                        "Market.ExchangeNotFound",
                        "The specified exchange was not found."));
            }

            if (!exchange.IsActive)
            {
                return Result<MarketId>.Fail(
                    new Error(
                        "Market.ExchangeInactive",
                        "The specified exchange is inactive and cannot be assigned to a new market."));
            }

            exchangeId = parsedExchangeId;
        }

        Market market = Market.Create(
            code,
            request.Name,
            exchangeId);

        await _marketRepository.AddAsync(
            market,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result<MarketId>.Success(market.Id);
    }
}
