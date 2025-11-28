using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to delete decks
/// </summary>
public record DeleteDecksRequestDto : AnkiConnectRequest
{
    public DeleteDecksRequestDto(IEnumerable<string> decks, bool cardsToo)
    {
        Action = "deleteDecks";
        Params = new DeleteDecksParams { Decks = decks, CardsToo = cardsToo };
    }
}