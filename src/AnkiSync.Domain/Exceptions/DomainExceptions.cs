namespace AnkiSync.Domain.Exceptions;

/// <summary>
/// Base exception for AnkiSync domain operations
/// </summary>
public class AnkiSyncException : Exception
{
    public AnkiSyncException(string message) : base(message)
    {
    }

    public AnkiSyncException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when Anki connection fails
/// </summary>
public class AnkiConnectionException : AnkiSyncException
{
    public AnkiConnectionException(string message) : base(message)
    {
    }

    public AnkiConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when flashcard validation fails
/// </summary>
public class ValidationException : AnkiSyncException
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

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

/// <summary>
/// Exception thrown when deck operations fail
/// </summary>
public class DeckException : AnkiSyncException
{
    public string? DeckName { get; }

    public DeckException(string message) : base(message)
    {
    }

    public DeckException(string message, string deckName) : base(message)
    {
        DeckName = deckName;
    }

    public DeckException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
