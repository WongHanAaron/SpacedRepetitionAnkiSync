using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for addTags
/// </summary>
public record AddTagsResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}