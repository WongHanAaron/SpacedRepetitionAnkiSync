using AnkiSync.Adapter.SpacedRepetitionNotes.Models;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Interface for inferring deck hierarchies
/// </summary>
public interface IDeckInferencer
{
    /// <summary>
    /// Groups cards into decks based on file paths and tags
    /// </summary>
    /// <param name="cards">The cards to group</param>
    /// <returns>The inferred decks</returns>
    IEnumerable<ParsedDeck> InferDecks(IEnumerable<ParsedCard> cards);
}

/// <summary>
/// Implementation of IDeckInferencer
/// </summary>
public class DeckInferencer : IDeckInferencer
{
    /// <summary>
    /// Groups cards into decks based on file paths and tags
    /// </summary>
    /// <param name="cards">The cards to group</param>
    /// <returns>The inferred decks</returns>
    public IEnumerable<ParsedDeck> InferDecks(IEnumerable<ParsedCard> cards)
    {
        // Group cards by inferred deck path
        var deckGroups = cards.GroupBy(card => InferDeckPath(card));

        foreach (var group in deckGroups)
        {
            yield return new ParsedDeck
            {
                DeckPath = group.Key,
                Cards = group
            };
        }
    }

    private string InferDeckPath(ParsedCard card)
    {
        // Use the directory structure to infer deck hierarchy
        var fileInfo = new FileInfo(card.SourceFilePath);
        var directory = fileInfo.Directory;

        if (directory == null)
        {
            return Path.GetFileNameWithoutExtension(card.SourceFilePath);
        }

        // Build hierarchy from directory names
        var pathParts = new List<string>();
        var currentDir = directory;

        while (currentDir != null && !string.IsNullOrEmpty(currentDir.Name))
        {
            pathParts.Insert(0, currentDir.Name);
            currentDir = currentDir.Parent;
        }

        // Add filename without extension
        pathParts.Add(Path.GetFileNameWithoutExtension(card.SourceFilePath));

        // Join with :: separator
        return string.Join("::", pathParts);
    }
}