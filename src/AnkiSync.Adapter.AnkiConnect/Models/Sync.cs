using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to sync Anki collection with AnkiWeb
/// </summary>
public record SyncRequestDto : AnkiConnectRequest
{
    public SyncRequestDto()
    {
        Action = "sync";
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for sync operation
/// </summary>
public record SyncResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}