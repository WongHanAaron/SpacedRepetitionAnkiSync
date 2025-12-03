using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to add a note
/// </summary>
public record AddNoteRequestDto : AnkiConnectRequest
{
    public AddNoteRequestDto(AnkiNote note)
    {
        Action = "addNote";
        Params = new AddNoteParams { Note = note };
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Parameters for add note request
/// </summary>
public record AddNoteParams
{
    [JsonPropertyName("note")]
    public required AnkiNote Note { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for add note
/// </summary>
public record AddNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public long? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}