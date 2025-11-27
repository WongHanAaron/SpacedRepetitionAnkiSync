namespace AnkiSync.Domain.Exceptions;

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