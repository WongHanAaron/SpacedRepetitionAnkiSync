using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Represents an Anki note to be created or updated
/// </summary>
public class AnkiNote
{
    /// <summary>
    /// The Anki deck name
    /// </summary>
    [JsonPropertyName("deckName")]
    public string DeckName { get; set; } = string.Empty;

    /// <summary>
    /// The Anki model name (e.g., "Basic", "Cloze")
    /// </summary>
    [JsonPropertyName("modelName")]
    public string ModelName { get; set; } = "Basic";

    /// <summary>
    /// The note fields (Front, Back, etc.)
    /// </summary>
    [JsonPropertyName("fields")]
    public Dictionary<string, string> Fields { get; set; } = new();
}