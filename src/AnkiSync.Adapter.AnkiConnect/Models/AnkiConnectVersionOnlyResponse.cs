namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for version/test connection - this endpoint returns a different format than other AnkiConnect endpoints
/// </summary>
public record AnkiConnectVersionOnlyResponse
{
    public required string ApiVersion { get; init; }
}