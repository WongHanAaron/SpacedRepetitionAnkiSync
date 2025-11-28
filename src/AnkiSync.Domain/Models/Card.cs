namespace AnkiSync.Domain;

/// <summary>
/// Base class for all card types
/// </summary>
public abstract class Card
{
    /// <summary>
    /// Unique identifier for the card
    /// </summary>
    public required string Id { get; set; }

    public required DateTimeOffset DateModified { get; set; }

    /// <summary>
    /// The type of the card
    /// </summary>
    public abstract CardType Type { get; }
}

/// <summary>
/// Enumeration of card types
/// </summary>
public enum CardType
{
    QuestionAnswer,
    Cloze
}