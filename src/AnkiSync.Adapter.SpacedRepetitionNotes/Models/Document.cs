namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Represents metadata and content of a parsed document
/// </summary>
public record Document
{
    /// <summary>
    /// The file path
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// When the file was last modified
    /// </summary>
    public required DateTime LastModified { get; init; }

    /// <summary>
    /// Tags found in the file
    /// </summary>
    public required Tag Tags { get; init; }

    /// <summary>
    /// The raw content of the file
    /// </summary>
    public required string Content { get; init; }
}