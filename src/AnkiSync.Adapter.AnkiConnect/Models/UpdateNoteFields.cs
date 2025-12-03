using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to update note fields
/// </summary>
public record UpdateNoteFieldsRequestDto : AnkiConnectRequest
{
    public UpdateNoteFieldsRequestDto(long noteId, Dictionary<string, string> fields)
    {
        Action = "updateNoteFields";
        Params = new UpdateNoteFieldsParams { Note = new UpdateNoteFieldsNote { Id = noteId, Fields = fields } };
    }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Parameters for updateNoteFields request
/// </summary>
public record UpdateNoteFieldsParams
{
    [JsonPropertyName("note")]
    public required UpdateNoteFieldsNote Note { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Note object for updateNoteFields request
/// </summary>
public record UpdateNoteFieldsNote
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }
    [JsonPropertyName("fields")]
    public required Dictionary<string, string> Fields { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

/// <summary>
/// Response for updateNoteFields
/// </summary>
public record UpdateNoteFieldsResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}