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