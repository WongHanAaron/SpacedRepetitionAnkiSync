using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for version check
/// </summary>
public record VersionResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}