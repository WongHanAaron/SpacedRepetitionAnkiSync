using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to create a deck
/// </summary>
public record CreateDeckRequestDto : AnkiConnectRequest
{
    public CreateDeckRequestDto(string deck)
    {
        Action = "createDeck";
        Params = new CreateDeckParams { Deck = deck };
    }
}