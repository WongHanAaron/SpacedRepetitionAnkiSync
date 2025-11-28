using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to get detailed information about notes
/// </summary>
public record NotesInfoRequestDto : AnkiConnectRequest
{
    public NotesInfoRequestDto(IEnumerable<long> notes)
    {
        Action = "notesInfo";
        Params = new NotesInfoParams { Notes = notes };
    }
}

/// <summary>
/// Parameters for notesInfo request
/// </summary>
public record NotesInfoParams
{
    [JsonPropertyName("notes")]
    public required IEnumerable<long> Notes { get; init; }
}

/// <summary>
/// Response for notesInfo
/// </summary>
public record NotesInfoResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public List<NoteInfo>? Result { get; init; }
}

/// <summary>
/// Information about a single note
/// </summary>
public record NoteInfo
{
    [JsonPropertyName("noteId")]
    public long NoteId { get; init; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; init; } = new();

    [JsonPropertyName("fields")]
    public Dictionary<string, NoteFieldInfo> Fields { get; init; } = new();

    [JsonPropertyName("modelName")]
    public string ModelName { get; init; } = string.Empty;

    [JsonPropertyName("cards")]
    public List<long> Cards { get; init; } = new();

    [JsonPropertyName("mod")]
    public long ModificationTimestamp { get; init; }

    /// <summary>
    /// Gets the modification date as a DateTimeOffset
    /// </summary>
    public DateTimeOffset DateModified => DateTimeOffset.FromUnixTimeSeconds(ModificationTimestamp);
}

/// <summary>
/// Information about a note field
/// </summary>
public record NoteFieldInfo
{
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; init; }
}