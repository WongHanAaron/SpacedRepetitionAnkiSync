namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Represents a parsed flashcard before domain conversion
/// </summary>
public record ParsedCard
{
    /// <summary>
    /// The front content of the card
    /// </summary>
    public required string Front { get; init; }

    /// <summary>
    /// The back content of the card
    /// </summary>
    public required string Back { get; init; }

    /// <summary>
    /// Tags associated with the card
    /// </summary>
    public required Tag Tags { get; init; }

    /// <summary>
    /// The file path where this card was found
    /// </summary>
    public required string SourceFilePath { get; init; }

    /// <summary>
    /// The type of card (Basic, Cloze, etc.)
    /// </summary>
    public required string CardType { get; init; }

    /// <summary>
    /// The answers for cloze cards (empty for other card types)
    /// </summary>
    public Dictionary<string, string> Answers { get; init; } = new();
}