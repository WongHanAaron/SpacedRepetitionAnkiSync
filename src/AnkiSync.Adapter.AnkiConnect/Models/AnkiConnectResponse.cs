using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Base response from AnkiConnect API
/// </summary>
public record AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
    
    [JsonPropertyName("error")]
    public object? Error { get; init; }
}