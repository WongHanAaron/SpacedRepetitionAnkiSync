namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to create a deck
/// </summary>
public record CreateDeckRequestDto : AnkiConnectRequest
{
    public CreateDeckRequestDto(string deck) : base("createDeck")
    {
        Params = new CreateDeckParams { Deck = deck };
    }

    public CreateDeckParams Params { get; }
}