namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to add a note
/// </summary>
public record AddNoteRequestDto : AnkiConnectRequest
{
    public AddNoteRequestDto(AnkiNoteDto note) : base("addNote")
    {
        Params = new AddNoteParams { Note = note };
    }

    public AddNoteParams Params { get; }
}