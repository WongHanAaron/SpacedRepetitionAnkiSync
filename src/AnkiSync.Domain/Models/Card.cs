namespace AnkiSync.Domain;

/// <summary>
/// Base class for all card types
/// </summary>
public abstract class Card
{
    /// <summary>
    /// The Anki card ID (assigned when the card exists in Anki)
    /// </summary>
    public long? Id { get; set; }

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