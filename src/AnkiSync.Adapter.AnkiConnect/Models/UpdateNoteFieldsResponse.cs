using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for updateNoteFields
/// </summary>
public record UpdateNoteFieldsResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}