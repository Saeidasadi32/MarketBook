// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Infrastructure
// Namespace : MarketBook.Infrastructure.Persistence.Repositories
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Application.Abstractions.Persistence;
using MarketBook.Domain.Portfolio.ValueObjects;
using MarketBook.Domain.PortfolioRiskEvaluation.Aggregates;
using MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects;
using MarketBook.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MarketBook.Infrastructure.Persistence.Repositories;

/// <summary>
/// EN: Provides EF persistence for portfolio risk evaluation snapshots.
/// FA: Ù…Ø§Ù†Ø¯Ú¯Ø§Ø±Ø³Ø§Ø²ÛŒ EF Ø¨Ø±Ø§ÛŒ SnapshotÙ‡Ø§ÛŒ Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ Ø±ÛŒØ³Ú© Ù¾Ø±ØªÙÙˆÛŒ.
/// </summary>
public sealed class PortfolioRiskEvaluationRepository
    : IPortfolioRiskEvaluationRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>EN: Initializes the repository. FA: Repository Ø±Ø§ Ù…Ù‚Ø¯Ø§Ø±Ø¯Ù‡ÛŒ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.</summary>
    public PortfolioRiskEvaluationRepository(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(
        PortfolioRiskEvaluation evaluation,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<PortfolioRiskEvaluation>()
            .AddAsync(evaluation, cancellationToken);

    /// <inheritdoc />
    public Task<PortfolioRiskEvaluation?> GetByIdAsync(
        PortfolioRiskEvaluationId id,
        CancellationToken cancellationToken = default)
        => _dbContext.Set<PortfolioRiskEvaluation>()
            .AsNoTracking()
            .Include(item => item.Rules)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<PortfolioRiskEvaluation>> GetHistoryAsync(
        PortfolioId portfolioId,
        CancellationToken cancellationToken = default)
        => await _dbContext.Set<PortfolioRiskEvaluation>()
            .AsNoTracking()
            .Include(item => item.Rules)
            .Where(item => item.PortfolioId == portfolioId)
            .OrderByDescending(item => item.EvaluatedOn)
            .ThenByDescending(item => item.Id)
            .ToArrayAsync(cancellationToken);
}