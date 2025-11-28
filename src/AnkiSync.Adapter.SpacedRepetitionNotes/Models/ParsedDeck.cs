using AnkiSync.Adapter.SpacedRepetitionNotes.Models;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Represents a parsed deck with hierarchical path information
/// </summary>
public record ParsedDeck
{
    /// <summary>
    /// The hierarchical path of the deck (e.g., "Parent::Child::DeckName")
    /// </summary>
    public required string DeckPath { get; init; }

    /// <summary>
    /// The cards in this deck
    /// </summary>
    public required IEnumerable<ParsedCard> Cards { get; init; }
}