namespace AnkiSync.Domain;

/// <summary>
/// Represents a question-answer flashcard
/// </summary>
public class QuestionAnswerCard : Card
{
    /// <summary>
    /// The question or front side of the flashcard
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// The answer or back side of the flashcard
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// The type of the card
    /// </summary>
    public override CardType Type => CardType.QuestionAnswer;
}