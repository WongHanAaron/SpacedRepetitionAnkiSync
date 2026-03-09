namespace AnkiSync.Domain;

/// <summary>
/// Represents a deck containing cards and sub-deck information
/// </summary>
public class Deck
{
    /// <summary>
    /// The deck identifier with hierarchical path support
    /// </summary>
    public required DeckId DeckId { get; set; }

    /// <summary>
    /// Whether the deck is a filtered (dynamic) deck in Anki.  Filtered decks have
    /// negative ids when queried via AnkiConnect.
    /// </summary>
    public bool IsFiltered { get; set; } = false;

    /// <summary>
    /// The name of the deck (derived from DeckId)
    /// </summary>
    public string Name => DeckId.Name;

    /// <summary>
    /// The cards in this deck
    /// </summary>
    public List<Card> Cards { get; set; } = new List<Card>();
}