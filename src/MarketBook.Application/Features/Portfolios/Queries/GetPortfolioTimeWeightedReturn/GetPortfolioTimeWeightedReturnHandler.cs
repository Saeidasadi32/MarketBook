// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Application
// Namespace : MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTranslatedNavAsOf;
using MarketBook.Domain.Common;
using MarketBook.Domain.Portfolio.Aggregates;
using MarketBook.Domain.Portfolio.Enums;
using MarketBook.Domain.Portfolio.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Application.Features.Portfolios.Queries.GetPortfolioTimeWeightedReturn;

/// <summary>
/// EN: Calculates historical TWR by splitting the period at external capital-flow instants.
/// FA: TWR تاریخی را با شکستن دوره در لحظه‌های جریان سرمایه خارجی محاسبه می‌کند.
/// </summary>
public sealed class GetPortfolioTimeWeightedReturnHandler
    : IRequestHandler<GetPortfolioTimeWeightedReturnQuery, Result<GetPortfolioTimeWeightedReturnResponse>>
{
    private readonly ISender _sender;
    private readonly IApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the handler. FA: Handler را مقداردهی می‌کند.</summary>
    public GetPortfolioTimeWeightedReturnHandler(ISender sender, IApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(dbContext);
        _sender = sender;
        _dbContext = dbContext;
    }

    /// <summary>EN: Handles the TWR query. FA: درخواست TWR را پردازش می‌کند.</summary>
    public async Task<Result<GetPortfolioTimeWeightedReturnResponse>> Handle(
        GetPortfolioTimeWeightedReturnQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.From >= request.To)
        {
            return Result<GetPortfolioTimeWeightedReturnResponse>.Fail(
                new Error("PortfolioTwr.InvalidPeriod", "From must be earlier than To."));
        }

        if (!PortfolioId.TryParse(request.PortfolioId, out PortfolioId? portfolioId) || portfolioId is null)
        {
            return Result<GetPortfolioTimeWeightedReturnResponse>.Fail(
                new Error("PortfolioTwr.InvalidPortfolioId", "The portfolio identifier is invalid."));
        }

        Result<GetPortfolioTranslatedNavAsOfResponse> beginningResult =
            await GetNavAsync(request.PortfolioId, request.From, cancellationToken);
        if (beginningResult.IsFailure)
        {
            return Result<GetPortfolioTimeWeightedReturnResponse>.Fail(beginningResult.Error);
        }

        GetPortfolioTranslatedNavAsOfResponse beginning = beginningResult.Value!;

        List<DateTimeOffset> boundaries = await _dbContext.Set<PortfolioCashTransaction>()
            .AsNoTracking()
            .Where(item => item.PortfolioId == portfolioId &&
                           item.OccurredOn > request.From &&
                           item.OccurredOn <= request.To &&
                           (item.Type == PortfolioCashTransactionType.Deposit ||
                            item.Type == PortfolioCashTransactionType.Withdrawal))
            .Select(item => item.OccurredOn)
            .Distinct()
            .OrderBy(item => item)
            .ToListAsync(cancellationToken);

        List<PortfolioTwrSegmentResponse> segments = [];
        DateTimeOffset segmentStart = request.From;
        NavPoint segmentBeginning = ToNavPoint(beginning);

        foreach (DateTimeOffset boundary in boundaries)
        {
            DateTimeOffset beforeFlow = boundary.AddTicks(-1);
            if (beforeFlow >= segmentStart)
            {
                Result<GetPortfolioTranslatedNavAsOfResponse> preResult =
                    await GetNavAsync(request.PortfolioId, beforeFlow, cancellationToken);
                if (preResult.IsFailure)
                {
                    return Result<GetPortfolioTimeWeightedReturnResponse>.Fail(preResult.Error);
                }
                segments.Add(CreateSegment(segmentStart, boundary, segmentBeginning, ToNavPoint(preResult.Value!)));
            }

            Result<GetPortfolioTranslatedNavAsOfResponse> postResult =
                await GetNavAsync(request.PortfolioId, boundary, cancellationToken);
            if (postResult.IsFailure)
            {
                return Result<GetPortfolioTimeWeightedReturnResponse>.Fail(postResult.Error);
            }
            segmentStart = boundary;
            segmentBeginning = ToNavPoint(postResult.Value!);
        }

        Result<GetPortfolioTranslatedNavAsOfResponse> endingResult =
            await GetNavAsync(request.PortfolioId, request.To, cancellationToken);
        if (endingResult.IsFailure)
        {
            return Result<GetPortfolioTimeWeightedReturnResponse>.Fail(endingResult.Error);
        }
        NavPoint ending = ToNavPoint(endingResult.Value!);

        if (segmentStart < request.To)
        {
            segments.Add(CreateSegment(segmentStart, request.To, segmentBeginning, ending));
        }

        bool isComplete = segments.Count > 0 && segments.All(item => item.IsComplete);
        bool isCalculable = isComplete && segments.All(item => item.IsCalculable);
        decimal? twr = null;
        if (isCalculable)
        {
            decimal growthFactor = 1m;
            foreach (PortfolioTwrSegmentResponse segment in segments)
            {
                growthFactor *= 1m + segment.Return!.Value;
            }
            twr = growthFactor - 1m;
        }

        return Result<GetPortfolioTimeWeightedReturnResponse>.Success(
            new GetPortfolioTimeWeightedReturnResponse(
                request.PortfolioId, beginning.BaseCurrencyId, request.From, request.To,
                boundaries.Count, isComplete, isCalculable, twr, segments));
    }

    private async Task<Result<GetPortfolioTranslatedNavAsOfResponse>> GetNavAsync(
        string portfolioId, DateTimeOffset asOf, CancellationToken cancellationToken)
        => await _sender.Send(new GetPortfolioTranslatedNavAsOfQuery(portfolioId, asOf), cancellationToken);

    private static NavPoint ToNavPoint(GetPortfolioTranslatedNavAsOfResponse response)
        => new(response.IsComplete && response.NetAssetValueBase.HasValue, response.NetAssetValueBase);

    private static PortfolioTwrSegmentResponse CreateSegment(
        DateTimeOffset start, DateTimeOffset end, NavPoint beginning, NavPoint ending)
    {
        bool complete = beginning.IsComplete && ending.IsComplete;
        bool calculable = complete && beginning.NetAssetValueBase.HasValue && beginning.NetAssetValueBase.Value > 0m;
        decimal? result = calculable && ending.NetAssetValueBase.HasValue
            ? (ending.NetAssetValueBase.Value / beginning.NetAssetValueBase!.Value) - 1m
            : null;
        return new PortfolioTwrSegmentResponse(
            start, end, beginning.NetAssetValueBase, ending.NetAssetValueBase, complete, calculable, result);
    }

    private sealed record NavPoint(bool IsComplete, decimal? NetAssetValueBase);
}
