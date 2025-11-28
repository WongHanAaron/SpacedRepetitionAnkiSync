using System.Text.Json.Serialization;

namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to find notes
/// </summary>
public record FindNotesRequestDto : AnkiConnectRequest
{
    public FindNotesRequestDto(string query)
    {
        Action = "findNotes";
        Params = new FindNotesParams { Query = query };
    }
}

/// <summary>
/// Parameters for find notes request
/// </summary>
public record FindNotesParams
{
    [JsonPropertyName("query")]
    public required string Query { get; init; }
}

/// <summary>
/// Response for find notes
/// </summary>
public record FindNotesResponse : AnkiConnectResponse
{
    [JsonPropertyName("result")]
    public List<long>? Result { get; init; }
}