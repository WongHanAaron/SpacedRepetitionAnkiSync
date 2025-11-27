namespace AnkiSync.Adapter.AnkiConnect.Models;

public record CreateDeckParams
{
    public required string Deck { get; init; }
}