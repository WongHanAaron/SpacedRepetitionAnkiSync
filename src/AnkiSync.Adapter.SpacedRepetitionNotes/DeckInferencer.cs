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
        // First, group cards by their source file
        var fileGroups = cards.GroupBy(card => card.SourceFilePath);
        
        // Then, group by the tags in each file
        var tagGroups = new Dictionary<string, ParsedDeck>();
        
        foreach (var fileGroup in fileGroups)
        {
            // Get the first card to extract tags (all cards in the same file share the same tags)
            var firstCard = fileGroup.First();
            
            // If the card has tags, use them; otherwise, fall back to file-based tags
            var tag = firstCard.Tags?.NestedTags?.Count > 0 
                ? firstCard.Tags 
                : InferDeckTag(firstCard);
                
            // Create a key for the tag to group by
            var tagKey = string.Join("/", tag.NestedTags ?? new List<string>());
            
            if (!tagGroups.TryGetValue(tagKey, out var deck))
            {
                deck = new ParsedDeck { Tag = tag, Cards = new List<ParsedCardBase>() };
                tagGroups[tagKey] = deck;
            }
            
            // Add all cards from this file to the deck
            ((List<ParsedCardBase>)deck.Cards).AddRange(fileGroup);
        }
        
        return tagGroups.Values;
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