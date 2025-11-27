namespace AnkiSync.Adapter.AnkiConnect.Models;

public record FindNotesParams
{
    public required string Query { get; init; }
}