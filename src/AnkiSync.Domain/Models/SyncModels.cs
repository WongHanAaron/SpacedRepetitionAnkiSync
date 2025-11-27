namespace AnkiSync.Domain;

/// <summary>
/// Result of a synchronization operation
/// </summary>
public class SyncResult
{
    /// <summary>
    /// Whether the sync operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of flashcards processed
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Number of flashcards successfully synced
    /// </summary>
    public int SyncedCount { get; set; }

    /// <summary>
    /// Number of flashcards that failed to sync
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Details of failed operations
    /// </summary>
    public List<SyncError> Errors { get; set; } = new();

    /// <summary>
    /// When the sync operation completed
    /// </summary>
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Duration of the sync operation
    /// </summary>
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Current synchronization status
/// </summary>
public class SyncStatus
{
    /// <summary>
    /// Whether the sync service is currently running
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// Last sync operation result
    /// </summary>
    public SyncResult? LastSyncResult { get; set; }

    /// <summary>
    /// When the last sync operation occurred
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// Number of flashcards currently tracked
    /// </summary>
    public int TrackedFlashcardsCount { get; set; }

    /// <summary>
    /// Connection status to Anki
    /// </summary>
    public AnkiConnectionStatus AnkiConnectionStatus { get; set; }
}

/// <summary>
/// Anki connection status
/// </summary>
public enum AnkiConnectionStatus
{
    /// <summary>
    /// Connection not tested
    /// </summary>
    Unknown,

    /// <summary>
    /// Successfully connected to Anki
    /// </summary>
    Connected,

    /// <summary>
    /// Failed to connect to Anki
    /// </summary>
    Disconnected,

    /// <summary>
    /// Anki is not running
    /// </summary>
    AnkiNotRunning
}

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

/// <summary>
/// Represents an Anki note to be created or updated
/// </summary>
public class AnkiNote
{
    /// <summary>
    /// The Anki deck name
    /// </summary>
    public string DeckName { get; set; } = string.Empty;

    /// <summary>
    /// The Anki model name (e.g., "Basic", "Cloze")
    /// </summary>
    public string ModelName { get; set; } = "Basic";

    /// <summary>
    /// The note fields (Front, Back, etc.)
    /// </summary>
    public Dictionary<string, string> Fields { get; set; } = new();

    /// <summary>
    /// Tags for the note
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Audio files attached to the note
    /// </summary>
    public List<AnkiAudio> Audio { get; set; } = new();

    /// <summary>
    /// Video files attached to the note
    /// </summary>
    public List<AnkiVideo> Video { get; set; } = new();

    /// <summary>
    /// Image files attached to the note
    /// </summary>
    public List<AnkiImage> Images { get; set; } = new();
}

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
