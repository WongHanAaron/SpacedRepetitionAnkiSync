using AnkiSync.Adapter.SpacedRepetitionNotes.Models;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Represents a parsed deck with hierarchical path information
/// </summary>
public record ParsedDeck
{
    /// <summary>
    /// The tag representing the deck hierarchy
    /// </summary>
    public required Tag Tag { get; init; }

    /// <summary>
    /// The cards in this deck
    /// </summary>
    public required IEnumerable<ParsedCardBase> Cards { get; init; }
}