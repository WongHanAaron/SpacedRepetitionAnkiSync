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