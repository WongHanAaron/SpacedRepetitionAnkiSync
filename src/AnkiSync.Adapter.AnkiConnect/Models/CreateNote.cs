using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to create a note object (using addNote action for now since createNote doesn't exist in AnkiConnect)
/// </summary>
public record CreateNoteRequestDto : AnkiConnectRequest
{
    public CreateNoteRequestDto(AnkiNote note)
    {
        Action = "addNote";
        Params = new CreateNoteParams { Note = note };
    }
}

/// <summary>
/// Parameters for createNote request
/// </summary>
public record CreateNoteParams
{
    [JsonPropertyName("note")]
    public required AnkiNote Note { get; init; }
}

/// <summary>
/// Response for createNote
/// </summary>
public record CreateNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }
}