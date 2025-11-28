namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response containing all tags from Anki
/// </summary>
public record GetTagsResponse(List<string> Tags);