// -----------------------------------------------------------------------------
// Project   : MarketBook (Intelligent Market Book System)
// Platform  : MarketBook Platform
// Layer     : Domain
// Namespace : MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects
//
// Copyright (c) Saeid Asadi. All rights reserved.
// Licensed under the MIT License.
// -----------------------------------------------------------------------------

using MarketBook.Domain.Common;
using NUlid;

namespace MarketBook.Domain.PortfolioRiskEvaluation.ValueObjects;

/// <summary>
/// EN: Identifies a persisted portfolio risk evaluation snapshot.
/// FA: Ø´Ù†Ø§Ø³Ù‡ ÛŒÚ© Snapshot Ø°Ø®ÛŒØ±Ù‡â€ŒØ´Ø¯Ù‡ Ø§Ø² Ø§Ø±Ø²ÛŒØ§Ø¨ÛŒ Ø±ÛŒØ³Ú© Ù¾Ø±ØªÙÙˆÛŒ.
/// </summary>
public sealed record PortfolioRiskEvaluationId : EntityId
{
    private PortfolioRiskEvaluationId(Ulid value) : base(value) { }

    /// <summary>EN: Creates a new identifier. FA: ÛŒÚ© Ø´Ù†Ø§Ø³Ù‡ Ø¬Ø¯ÛŒØ¯ Ø§ÛŒØ¬Ø§Ø¯ Ù…ÛŒâ€ŒÚ©Ù†Ø¯.</summary>
    public static PortfolioRiskEvaluationId New() => new(Ulid.NewUlid());

    /// <summary>EN: Parses an identifier. FA: Ø´Ù†Ø§Ø³Ù‡ Ø±Ø§ Parse Ù…ÛŒâ€ŒÚ©Ù†Ø¯.</summary>
    public static PortfolioRiskEvaluationId Parse(string value)
    {
        Guard.AgainstNullOrWhiteSpace(value, nameof(PortfolioRiskEvaluationId));

        if (Ulid.TryParse(value, out Ulid ulid))
        {
            return new PortfolioRiskEvaluationId(ulid);
        }

        throw new DomainException(
            new Error(
                "PortfolioRiskEvaluationId.InvalidFormat",
                "The portfolio risk evaluation identifier is invalid."));
    }

    /// <summary>EN: Tries to parse an identifier. FA: ØªÙ„Ø§Ø´ Ù…ÛŒâ€ŒÚ©Ù†Ø¯ Ø´Ù†Ø§Ø³Ù‡ Ø±Ø§ Parse Ú©Ù†Ø¯.</summary>
    public static bool TryParse(string? value, out PortfolioRiskEvaluationId? id)
    {
        id = null;

        if (string.IsNullOrWhiteSpace(value) ||
            !Ulid.TryParse(value, out Ulid ulid))
        {
            return false;
        }

        id = new PortfolioRiskEvaluationId(ulid);
        return true;
    }
}