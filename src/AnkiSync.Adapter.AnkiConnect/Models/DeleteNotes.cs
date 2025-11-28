using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to delete notes
/// </summary>
public record DeleteNotesRequestDto : AnkiConnectRequest
{
    public DeleteNotesRequestDto(IEnumerable<long> notes)
    {
        Action = "deleteNotes";
        Params = new DeleteNotesParams { Notes = notes };
    }
}

/// <summary>
/// Parameters for delete notes request
/// </summary>
public record DeleteNotesParams
{
    [JsonPropertyName("notes")]
    public required IEnumerable<long> Notes { get; init; }
}

/// <summary>
/// Response for delete notes
/// </summary>
public record DeleteNotesResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public int? Result { get; init; }
}