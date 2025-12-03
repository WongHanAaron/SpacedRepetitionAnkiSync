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

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for version check
/// </summary>
public record VersionResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}