using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to check if a note can be added
/// </summary>
public record CanAddNoteRequestDto : AnkiConnectRequest
{
    public CanAddNoteRequestDto(AnkiNote note)
    {
        Action = "canAddNote";
        Params = new CanAddNoteParams { Note = note };
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Parameters for canAddNote request
/// </summary>
public record CanAddNoteParams
{
    [JsonPropertyName("note")]
    public required AnkiNote Note { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for canAddNote
/// </summary>
public record CanAddNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public bool? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}