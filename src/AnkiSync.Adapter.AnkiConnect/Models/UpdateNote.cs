using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to update a note
/// </summary>
public record UpdateNoteRequestDto : AnkiConnectRequest
{
    public UpdateNoteRequestDto(long noteId, Dictionary<string, string> fields)
    {
        Action = "updateNoteFields";
        Params = new UpdateNoteParams
        {
            Note = new UpdateNoteData { Id = noteId, Fields = fields }
        };
    }
}

/// <summary>
/// Parameters for update note request
/// </summary>
public record UpdateNoteParams
{
    [JsonPropertyName("note")]
    public required UpdateNoteData Note { get; init; }
}

/// <summary>
/// Note data for update note request
/// </summary>
public record UpdateNoteData
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("fields")]
    public required Dictionary<string, string> Fields { get; init; }
}

/// <summary>
/// Response for update note
/// </summary>
public record UpdateNoteResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public object? Result { get; init; }
}