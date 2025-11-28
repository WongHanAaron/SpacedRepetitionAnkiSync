using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Implementation of ICardSourceRepository for Spaced Repetition Notes
/// </summary>
public class SpacedRepetitionNotesRepository : ICardSourceRepository, IDisposable
{
    private readonly IFileParser _fileParser;
    private readonly ICardExtractor _cardExtractor;
    private readonly IDeckInferencer _deckInferencer;
    private readonly IFileSystem _fileSystem;
    private FileSystemWatcher? _fileSystemWatcher;

    /// <summary>
    /// Initializes a new instance of SpacedRepetitionNotesRepository
    /// </summary>
    public SpacedRepetitionNotesRepository(
        IFileParser fileParser,
        ICardExtractor cardExtractor,
        IDeckInferencer deckInferencer,
        IFileSystem fileSystem)
    {
        _fileParser = fileParser;
        _cardExtractor = cardExtractor;
        _deckInferencer = deckInferencer;
        _fileSystem = fileSystem;
    }

    /// <summary>
    /// Retrieves all flashcards from the specified file paths
    /// </summary>
    public async Task<IEnumerable<Deck>> GetCardsFromFiles(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
    {
        var allCards = new List<ParsedCardBase>();

        foreach (var filePath in filePaths)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                // Read file content and metadata
                var fileInfo = _fileSystem.GetFileInfo(filePath);
                var content = await _fileSystem.ReadAllTextAsync(filePath);

                // Parse the content
                var document = await _fileParser.ParseContentAsync(filePath, content, fileInfo.LastWriteTimeUtc);
                var cards = _cardExtractor.ExtractCards(document);
                allCards.AddRange(cards);
            }
            catch (Exception)
            {
                // Skip files that can't be parsed
                continue;
            }
        }

        var decks = _deckInferencer.InferDecks(allCards);
        return ConvertToDomainDecks(decks);
    }

    /// <summary>
    /// Retrieves all flashcards from the specified directories
    /// </summary>
    public async Task<IEnumerable<Deck>> GetCardsFromDirectories(IEnumerable<string> directories, CancellationToken cancellationToken = default)
    {
        var filePaths = new List<string>();

        foreach (var directory in directories)
        {
            if (_fileSystem.DirectoryExists(directory))
            {
                // Find all markdown files
                var markdownFiles = _fileSystem.GetFiles(directory, "*.md", SearchOption.AllDirectories);
                filePaths.AddRange(markdownFiles);
            }
        }

        return await GetCardsFromFiles(filePaths, cancellationToken);
    }

    /// <summary>
    /// Subscribes to changes in the specified directory and raises CardsUpdated event when files change
    /// </summary>
    /// <param name="directoryPath">The directory path to monitor for changes</param>
    public void SubscribeToDirectoryChanges(string directoryPath)
    {
        if (_fileSystemWatcher != null)
        {
            _fileSystemWatcher.Dispose();
        }

        _fileSystemWatcher = new FileSystemWatcher(directoryPath)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            Filter = "*.md",
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        _fileSystemWatcher.Changed += OnFileChanged;
        _fileSystemWatcher.Created += OnFileChanged;
        _fileSystemWatcher.Deleted += OnFileChanged;
        _fileSystemWatcher.Renamed += OnFileRenamed;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        CardsUpdated?.Invoke(this, new CardsUpdatedEventArgs([e.FullPath]));
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        var updatedPaths = new List<string>();
        if (!string.IsNullOrEmpty(e.OldFullPath))
        {
            updatedPaths.Add(e.OldFullPath);
        }
        if (!string.IsNullOrEmpty(e.FullPath))
        {
            updatedPaths.Add(e.FullPath);
        }
        CardsUpdated?.Invoke(this, new CardsUpdatedEventArgs(updatedPaths));
    }

    /// <summary>
    /// Event raised when cards in the sources might have been updated
    /// </summary>
#pragma warning disable CS0067
    public event EventHandler<CardsUpdatedEventArgs>? CardsUpdated;
#pragma warning restore CS0067

    private IEnumerable<Deck> ConvertToDomainDecks(IEnumerable<ParsedDeck> parsedDecks)
    {
        foreach (var parsedDeck in parsedDecks)
        {
            var deck = new Deck
            {
                DeckId = DeckId.FromPath([..parsedDeck.Tag.NestedTags]),
                Cards = parsedDeck.Cards.Select(ConvertToDomainCard).ToList()
            };
            yield return deck;
        }
    }

    private Card ConvertToDomainCard(ParsedCardBase parsedCard)
    {
        // Generate a unique ID for the card
        var cardId = Guid.NewGuid().ToString();

        // Determine card type and create appropriate domain card
        if (parsedCard is ParsedClozeCard clozeCard)
        {
            return new ClozeCard
            {
                Id = cardId,
                DateModified = DateTimeOffset.UtcNow,
                Text = clozeCard.Text,
                Answers = clozeCard.Answers
                // Tags are not part of the domain model yet
            };
        }
        else if (parsedCard is ParsedQuestionAnswerCard qaCard)
        {
            return new QuestionAnswerCard
            {
                Id = cardId,
                DateModified = DateTimeOffset.UtcNow,
                Question = qaCard.Question,
                Answer = qaCard.Answer
                // Tags are not part of the domain model yet
            };
        }
        else
        {
            throw new InvalidOperationException($"Unknown card type: {parsedCard.GetType()}");
        }
    }

    /// <summary>
    /// Disposes the repository and cleans up resources
    /// </summary>
    public void Dispose()
    {
        _fileSystemWatcher?.Dispose();
    }
}