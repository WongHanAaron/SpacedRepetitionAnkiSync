namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for find notes
/// </summary>
public record AnkiConnectFindNotesResponse : AnkiConnectResponse<List<long>>;