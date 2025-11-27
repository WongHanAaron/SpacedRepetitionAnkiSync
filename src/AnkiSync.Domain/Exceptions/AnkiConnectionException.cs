namespace AnkiSync.Domain.Exceptions;

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