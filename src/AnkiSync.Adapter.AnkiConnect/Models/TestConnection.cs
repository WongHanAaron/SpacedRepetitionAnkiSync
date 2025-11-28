using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to test connection to Anki
/// </summary>
public record TestConnectionRequestDto : AnkiConnectRequest
{
    public TestConnectionRequestDto()
    {
        Action = "version";
    }
}

/// <summary>
/// Response for version check
/// </summary>
public record VersionResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}