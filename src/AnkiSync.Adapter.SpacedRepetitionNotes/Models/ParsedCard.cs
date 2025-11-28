namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Base class for parsed flashcards
/// </summary>
public abstract record ParsedCardBase
{
    /// <summary>
    /// Tags associated with the card
    /// </summary>
    public required Tag Tags { get; init; }

    /// <summary>
    /// The file path where this card was found
    /// </summary>
    public required string SourceFilePath { get; init; }
}

/// <summary>
/// Represents a parsed question-answer flashcard
/// </summary>
public record ParsedQuestionAnswerCard : ParsedCardBase
{
    /// <summary>
    /// The question or front content of the card
    /// </summary>
    public required string Question { get; init; }

    /// <summary>
    /// The answer or back content of the card
    /// </summary>
    public required string Answer { get; init; }
}

/// <summary>
/// Represents a parsed cloze flashcard
/// </summary>
public record ParsedClozeCard : ParsedCardBase
{
    /// <summary>
    /// The text with named placeholders for answers
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Dictionary mapping placeholder names to their answer values
    /// </summary>
    public required Dictionary<string, string> Answers { get; init; }
}