using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for getTags
/// </summary>
public record GetTagsResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public List<string>? Result { get; init; }
}