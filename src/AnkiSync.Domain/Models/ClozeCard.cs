namespace AnkiSync.Domain;

/// <summary>
/// Represents a cloze deletion flashcard
/// </summary>
public class ClozeCard : Card
{
    /// <summary>
    /// The text with named placeholders for answers (e.g., "The capital of {country} is {city}")
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Dictionary mapping placeholder names to their answer values
    /// </summary>
    public Dictionary<string, string> Answers { get; set; } = new();

    /// <summary>
    /// The type of the card
    /// </summary>
    public override CardType Type => CardType.Cloze;
}