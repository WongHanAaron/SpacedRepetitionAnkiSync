namespace AnkiSync.Adapter.AnkiConnect.Models;

using System.Text.Json.Serialization;

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
}

/// <summary>
/// Parameters for updateNoteFields request
/// </summary>
public record UpdateNoteFieldsParams
{
    [JsonPropertyName("note")]
    public required UpdateNoteFieldsNote Note { get; init; }
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
}