namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to find notes
/// </summary>
public record FindNotesRequestDto : AnkiConnectRequest
{
    public FindNotesRequestDto(string query) : base("findNotes")
    {
        Params = new FindNotesParams { Query = query };
    }

    public FindNotesParams Params { get; }
}