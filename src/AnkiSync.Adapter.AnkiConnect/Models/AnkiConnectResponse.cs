using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Base response from AnkiConnect API
/// </summary>
public record AnkiConnectResponse
{
    [JsonPropertyName("error")]
    public string? Error { get; init; }
}