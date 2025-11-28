namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Response for version/test connection - this endpoint returns a different format than other AnkiConnect endpoints
/// </summary>
public record VersionOnlyResponse
{
    public required string ApiVersion { get; init; }
}