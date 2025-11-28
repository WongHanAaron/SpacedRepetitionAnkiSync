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
    IEnumerable<ParsedDeck> InferDecks(IEnumerable<ParsedCardBase> cards);
}

/// <summary>
/// Implementation of IDeckInferencer
/// </summary>
public class DeckInferencer : IDeckInferencer
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Creates a new instance of DeckInferencer
    /// </summary>
    /// <param name="fileSystem">The file system abstraction to use</param>
    public DeckInferencer(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    /// <summary>
    /// Groups cards into decks based on file paths and tags
    /// </summary>
    /// <param name="cards">The cards to group</param>
    /// <returns>The inferred decks</returns>
    public IEnumerable<ParsedDeck> InferDecks(IEnumerable<ParsedCardBase> cards)
    {
        // Group cards by inferred deck tag
        var deckGroups = cards.GroupBy(card => InferDeckTag(card));

        foreach (var group in deckGroups)
        {
            yield return new ParsedDeck
            {
                Tag = group.Key,
                Cards = group
            };
        }
    }

    private Tag InferDeckTag(ParsedCardBase card)
    {
        // Use the directory structure to infer deck hierarchy
        var fileInfo = _fileSystem.GetFileInfo(card.SourceFilePath);
        var directory = fileInfo.Directory;

        if (directory == null)
        {
            return new Tag { NestedTags = new List<string> { _fileSystem.GetFileNameWithoutExtension(card.SourceFilePath) } };
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
        pathParts.Add(_fileSystem.GetFileNameWithoutExtension(card.SourceFilePath));

        // Return as Tag
        return new Tag { NestedTags = pathParts };
    }
}