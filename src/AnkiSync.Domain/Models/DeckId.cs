namespace AnkiSync.Domain;

/// <summary>
/// Represents a deck identifier
/// </summary>
public record DeckId
{
    /// <summary>
    /// The name of the deck
    /// </summary>
    public string Name { get; init; } = string.Empty;
}