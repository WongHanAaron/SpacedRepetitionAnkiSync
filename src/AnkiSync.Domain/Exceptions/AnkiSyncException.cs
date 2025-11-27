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