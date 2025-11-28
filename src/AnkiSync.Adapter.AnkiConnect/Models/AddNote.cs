using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to add a note
/// </summary>
public record AddNoteRequestDto : AnkiConnectRequest
{
    public AddNoteRequestDto(AnkiNoteDto note)
    {
        Action = "addNote";
        Params = new AddNoteParams { Note = note };
    }
}

/// <summary>
/// Parameters for add note request
/// </summary>
public record AddNoteParams
{
    [JsonPropertyName("note")]
    public required AnkiNoteDto Note { get; init; }
}

/// <summary>
/// Response for add note
/// </summary>
public record AddNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }
}