namespace AnkiSync.Domain;

/// <summary>
/// Represents a video file attachment
/// </summary>
public class AnkiVideo
{
    /// <summary>
    /// URL or path to the video file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Filename for the video
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Whether to skip hashing the file
    /// </summary>
    public bool SkipHash { get; set; }

    /// <summary>
    /// Field name to attach the video to
    /// </summary>
    public string Fields { get; set; } = string.Empty;
}