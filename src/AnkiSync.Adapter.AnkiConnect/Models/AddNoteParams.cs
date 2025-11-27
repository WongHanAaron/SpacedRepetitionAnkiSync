namespace AnkiSync.Adapter.AnkiConnect.Models;

public record AddNoteParams
{
    public required AnkiNoteDto Note { get; init; }
}