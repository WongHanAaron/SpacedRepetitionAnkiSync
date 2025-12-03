namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Anki-specific version of QuestionAnswerCard with Anki ID
/// </summary>
public class AnkiQuestionAnswerCard : AnkiSync.Domain.QuestionAnswerCard
{
    /// <summary>
    /// The Anki-specific ID for this card
    /// </summary>
    public long Id { get; set; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}