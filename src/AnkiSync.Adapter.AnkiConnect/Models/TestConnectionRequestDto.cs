namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to test connection to Anki
/// </summary>
public record TestConnectionRequestDto : AnkiConnectRequest
{
    public TestConnectionRequestDto()
    {
        Action = "version";
    }
}