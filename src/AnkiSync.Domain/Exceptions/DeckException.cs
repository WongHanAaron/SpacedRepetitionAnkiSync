namespace AnkiSync.Domain.Exceptions;

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