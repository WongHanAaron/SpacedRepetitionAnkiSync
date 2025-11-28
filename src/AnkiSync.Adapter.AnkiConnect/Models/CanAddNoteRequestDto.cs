namespace AnkiSync.Adapter.AnkiConnect.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Request to check if a note can be added
/// </summary>
public record CanAddNoteRequestDto : AnkiConnectRequest
{
    public CanAddNoteRequestDto(AnkiNoteDto note)
    {
        Action = "canAddNote";
        Params = new CanAddNoteParams { Note = note };
    }
}

/// <summary>
/// Parameters for canAddNote request
/// </summary>
public record CanAddNoteParams
{
    [JsonPropertyName("note")]
    public required AnkiNoteDto Note { get; init; }
}