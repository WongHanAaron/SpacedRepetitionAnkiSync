namespace AnkiSync.Domain;

/// <summary>
/// Represents a flashcard with question, answer, and metadata
/// </summary>
public class Flashcard
{
    /// <summary>
    /// Unique identifier for the flashcard
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The question or front side of the flashcard
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// The answer or back side of the flashcard
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// The type of flashcard (Basic, Cloze, etc.)
    /// </summary>
    public FlashcardType Type { get; set; } = FlashcardType.Basic;

    /// <summary>
    /// Tags associated with this flashcard
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// The source file path where this flashcard was found
    /// </summary>
    public string SourceFile { get; set; } = string.Empty;

    /// <summary>
    /// Line number in the source file where this flashcard starts
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// When this flashcard was last modified in the source
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Anki note ID if this flashcard has been synced
    /// </summary>
    public string? AnkiNoteId { get; set; }

    /// <summary>
    /// The inferred Anki deck for this flashcard
    /// </summary>
    public string InferredDeck { get; set; } = string.Empty;
}

/// <summary>
/// Types of flashcards supported by Obsidian
/// </summary>
public enum FlashcardType
{
    /// <summary>
    /// Basic flashcard: Question::Answer
    /// </summary>
    Basic,

    /// <summary>
    /// Bidirectional flashcard: Term:::Definition
    /// </summary>
    Bidirectional,

    /// <summary>
    /// Cloze deletion: Text with ==hidden== content
    /// </summary>
    Cloze,

    /// <summary>
    /// Multi-line basic: Question?\nAnswer
    /// </summary>
    MultiLineBasic,

    /// <summary>
    /// Multi-line bidirectional: Question??\nAnswer
    /// </summary>
    MultiLineBidirectional
}
