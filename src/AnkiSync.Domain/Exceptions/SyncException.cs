namespace AnkiSync.Domain.Exceptions;

/// <summary>
/// Exception thrown when sync operation fails
/// </summary>
public class SyncException : AnkiSyncException
{
    public string? FlashcardId { get; }

    public SyncException(string message) : base(message)
    {
    }

    public SyncException(string message, string flashcardId) : base(message)
    {
        FlashcardId = flashcardId;
    }

    public SyncException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}