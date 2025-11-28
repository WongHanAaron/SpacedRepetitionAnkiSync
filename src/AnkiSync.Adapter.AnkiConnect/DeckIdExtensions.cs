using AnkiSync.Domain;

namespace AnkiSync.Adapter.AnkiConnect;

/// <summary>
/// Extension methods for converting DeckId to/from Anki's deck naming format
/// </summary>
public static class DeckIdExtensions
{
    /// <summary>
    /// Converts a DeckId to Anki's full deck name format (using "::" as separator)
    /// </summary>
    public static string ToAnkiDeckName(this DeckId deckId)
    {
        if (deckId.Parents.Count == 0)
        {
            return deckId.Name;
        }
        return $"{string.Join("::", deckId.Parents)}::{deckId.Name}";
    }

    /// <summary>
    /// Creates a DeckId from Anki's full deck name format (using "::" as separator)
    /// </summary>
    public static DeckId FromAnkiDeckName(string ankiDeckName)
    {
        if (string.IsNullOrEmpty(ankiDeckName))
        {
            return new DeckId();
        }

        var parts = ankiDeckName.Split("::", StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return new DeckId();
        }

        return new DeckId
        {
            Name = parts.Last(),
            Parents = parts.Take(parts.Length - 1).ToList()
        };
    }
}
