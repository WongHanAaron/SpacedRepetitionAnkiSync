namespace AnkiSync.Adapter.AnkiConnect.Models;

public record UpdateNoteParams
{
    public required long Id { get; init; }
    public required Dictionary<string, string> Fields { get; init; }
}