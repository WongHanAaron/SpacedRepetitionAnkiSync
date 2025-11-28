namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Request to get all tags
/// </summary>
public record GetTagsRequestDto : AnkiConnectRequest
{
    public GetTagsRequestDto()
    {
        Action = "getTags";
        Params = new { };
    }
}