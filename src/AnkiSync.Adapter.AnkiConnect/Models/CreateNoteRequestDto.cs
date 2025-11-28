namespace AnkiSync.Adapter.AnkiConnect.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Request to create a note object (using addNote action for now since createNote doesn't exist in AnkiConnect)
/// </summary>
public record CreateNoteRequestDto : AnkiConnectRequest
{
    public CreateNoteRequestDto(AnkiNoteDto note)
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
    public required AnkiNoteDto Note { get; init; }
}