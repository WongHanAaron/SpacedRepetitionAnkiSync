namespace AnkiSync.Domain;

/// <summary>
/// Represents an image file attachment
/// </summary>
public class AnkiImage
{
    /// <summary>
    /// URL or path to the image file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Filename for the image
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Whether to skip hashing the file
    /// </summary>
    public bool SkipHash { get; set; }

    /// <summary>
    /// Field name to attach the image to
    /// </summary>
    public string Fields { get; set; } = string.Empty;
}