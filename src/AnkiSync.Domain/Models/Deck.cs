namespace AnkiSync.Domain;

/// <summary>
/// Represents a deck containing cards and sub-deck information
/// </summary>
public class Deck
{
    /// <summary>
    /// Unique identifier for the deck
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The deck identifier with hierarchical path support
    /// </summary>
    public required DeckId DeckId { get; set; }

    /// <summary>
    /// The name of the deck (derived from DeckId)
    /// </summary>
    public string Name => DeckId.Name;

    /// <summary>
    /// The cards in this deck
    /// </summary>
    public List<Card> Cards { get; set; } = new List<Card>();
}