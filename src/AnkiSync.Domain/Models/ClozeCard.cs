namespace AnkiSync.Domain;

/// <summary>
/// Represents a cloze deletion flashcard
/// </summary>
public class ClozeCard : Card
{
    /// <summary>
    /// The text with cloze deletions (e.g., "The capital of {{c1::France}} is {{c1::Paris}}")
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The type of the card
    /// </summary>
    public override CardType Type => CardType.Cloze;
}