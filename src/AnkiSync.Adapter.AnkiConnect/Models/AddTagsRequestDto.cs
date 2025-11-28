namespace AnkiSync.Adapter.AnkiConnect.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Request to add/remove tags from notes
/// </summary>
public record AddTagsRequestDto : AnkiConnectRequest
{
    public AddTagsRequestDto(IEnumerable<long> notes, IEnumerable<string> tags, bool add)
    {
        Action = "addTags";
        Params = new AddTagsParams { Notes = notes, Tags = string.Join(" ", tags) };
    }
}

/// <summary>
/// Parameters for addTags request
/// </summary>
public record AddTagsParams
{
    [JsonPropertyName("notes")]
    public required IEnumerable<long> Notes { get; init; }
    [JsonPropertyName("tags")]
    public required string Tags { get; init; }
}