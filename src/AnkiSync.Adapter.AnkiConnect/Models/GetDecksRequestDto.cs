namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to get all decks
/// </summary>
public record GetDecksRequestDto : AnkiConnectRequest
{
    public GetDecksRequestDto() : base("deckNames") { }
}