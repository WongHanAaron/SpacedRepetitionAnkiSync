using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using System.IO.Abstractions;

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
        // We now treat each card independently.  Previous logic grouped by
        // source file and only looked at the first card's tags, which meant
        // that two cards in the same file with different tags ended up in the
        // same deck.  The bug report pointed out exactly that scenario.
        
        var tagGroups = new Dictionary<string, ParsedDeck>();

        foreach (var card in cards)
        {
            // prefer explicit tags on the card, fall back to path-derived tag
            var tag = card.Tags?.NestedTags?.Count > 0
                ? card.Tags
                : InferDeckTag(card);

            var tagKey = string.Join("/", tag.NestedTags ?? new List<string>());

            if (!tagGroups.TryGetValue(tagKey, out var deck))
            {
                deck = new ParsedDeck { Tag = tag, Cards = new List<ParsedCardBase>() };
                tagGroups[tagKey] = deck;
            }

            ((List<ParsedCardBase>)deck.Cards).Add(card);
        }

        return tagGroups.Values;
    }

    private Tag InferDeckTag(ParsedCardBase card)
    {
        // Use the directory structure to infer deck hierarchy
        var fileInfo = _fileSystem.FileInfo.New(card.SourceFilePath);
        var directory = fileInfo.Directory;

        if (directory == null)
        {
            return new Tag { NestedTags = new List<string> { _fileSystem.Path.GetFileNameWithoutExtension(card.SourceFilePath) } };
        }

        // Build hierarchy from directory names
        var pathParts = new List<string>();
        var currentDir = directory;

        // walk up the directory tree, but stop before the drive/root itself
        while (currentDir != null && !string.IsNullOrEmpty(currentDir.Name) && currentDir.Parent != null)
        {
            pathParts.Insert(0, currentDir.Name);
            currentDir = currentDir.Parent;
        }

        // Return as Tag
        return new Tag { NestedTags = pathParts };
    }
}