namespace AnkiSync.Domain;

/// <summary>
/// Represents an Anki note to be created or updated
/// </summary>
public class AnkiNote
{
    /// <summary>
    /// The Anki deck name
    /// </summary>
    public string DeckName { get; set; } = string.Empty;

    /// <summary>
    /// The Anki model name (e.g., "Basic", "Cloze")
    /// </summary>
    public string ModelName { get; set; } = "Basic";

    /// <summary>
    /// The note fields (Front, Back, etc.)
    /// </summary>
    public Dictionary<string, string> Fields { get; set; } = new();

    /// <summary>
    /// Tags for the note
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Audio files attached to the note
    /// </summary>
    public List<AnkiAudio> Audio { get; set; } = new();

    /// <summary>
    /// Video files attached to the note
    /// </summary>
    public List<AnkiVideo> Video { get; set; } = new();

    /// <summary>
    /// Image files attached to the note
    /// </summary>
    public List<AnkiImage> Images { get; set; } = new();
}