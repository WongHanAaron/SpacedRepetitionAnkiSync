using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to get all decks along with their numeric IDs.
/// </summary>
public record GetDecksWithIdsRequestDto : AnkiConnectRequest
{
    public GetDecksWithIdsRequestDto()
    {
        Action = "deckNamesAndIds";
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}

/// <summary>
/// Response containing a map of deck name &lt;-&gt; deck id.  Filtered decks
/// are indicated by a negative id value.
/// </summary>
public record DeckNamesAndIdsResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public Dictionary<string, long>? Result { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}