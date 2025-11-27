namespace AnkiSync.Domain;

/// <summary>
/// Represents an audio file attachment
/// </summary>
public class AnkiAudio
{
    /// <summary>
    /// URL or path to the audio file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Filename for the audio
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Whether to skip hashing the file
    /// </summary>
    public bool SkipHash { get; set; }

    /// <summary>
    /// Field name to attach the audio to
    /// </summary>
    public string Fields { get; set; } = string.Empty;
}