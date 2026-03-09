using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

public record CreateFilteredDeckParams
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("search")]
    public string Search { get; init; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; init; }

    [JsonPropertyName("fullSearch")]
    public bool FullSearch { get; init; }
}

public record CreateFilteredDeckRequestDto : AnkiConnectRequest
{
    public CreateFilteredDeckRequestDto(string deckName, string search, int order, bool fullSearch = false)
    {
        Action = "createFilteredDeck";
        Params = new CreateFilteredDeckParams
        {
            Name = deckName,
            Search = search,
            Order = order,
            FullSearch = fullSearch
        };
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}