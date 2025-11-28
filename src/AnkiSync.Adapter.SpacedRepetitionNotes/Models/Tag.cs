namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Represents a tag with nested tag support
/// </summary>
public record Tag
{
    /// <summary>
    /// The nested tag components
    /// </summary>
    public required IReadOnlyList<string> NestedTags { get; init; } = new List<string>();
}