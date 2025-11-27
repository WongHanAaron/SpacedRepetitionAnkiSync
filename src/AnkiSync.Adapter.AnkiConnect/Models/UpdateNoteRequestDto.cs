namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to update a note
/// </summary>
public record UpdateNoteRequestDto : AnkiConnectRequest
{
    public UpdateNoteRequestDto(long noteId, Dictionary<string, string> fields) : base("updateNoteFields")
    {
        Params = new UpdateNoteParams { Id = noteId, Fields = fields };
    }

    public UpdateNoteParams Params { get; }
}