using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to delete notes
/// </summary>
public record DeleteNotesRequestDto : AnkiConnectRequest
{
    public DeleteNotesRequestDto(IEnumerable<long> notes)
    {
        Action = "deleteNotes";
        Params = new DeleteNotesParams { Notes = notes };
    }
}