namespace AnkiSync.Domain;

/// <summary>
/// Details of a sync operation error
/// </summary>
public class SyncError
{
    /// <summary>
    /// The flashcard that failed
    /// </summary>
    public string FlashcardId { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error code or type
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// When the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}