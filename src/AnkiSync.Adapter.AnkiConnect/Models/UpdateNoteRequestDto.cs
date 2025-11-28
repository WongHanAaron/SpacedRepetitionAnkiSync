using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to update a note
/// </summary>
public record UpdateNoteRequestDto : AnkiConnectRequest
{
    public UpdateNoteRequestDto(long noteId, Dictionary<string, string> fields)
    {
        Action = "updateNoteFields";
        Params = new UpdateNoteParams
        {
            Note = new UpdateNoteData { Id = noteId, Fields = fields }
        };
    }
}