using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SpacedRepetitionNotesRepository> _logger;
    private FileSystemWatcher? _fileSystemWatcher;

    /// <summary>
    /// Initializes a new instance of SpacedRepetitionNotesRepository
    /// </summary>
    public SpacedRepetitionNotesRepository(
        IFileParser fileParser,
        ICardExtractor cardExtractor,
        IDeckInferencer deckInferencer,
        IFileSystem fileSystem,
        ILogger<SpacedRepetitionNotesRepository> logger)
    {
        _fileParser = fileParser ?? throw new ArgumentNullException(nameof(fileParser));
        _cardExtractor = cardExtractor ?? throw new ArgumentNullException(nameof(cardExtractor));
        _deckInferencer = deckInferencer ?? throw new ArgumentNullException(nameof(deckInferencer));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all flashcards from the specified file paths
    /// </summary>
    public async Task<IEnumerable<Deck>> GetCardsFromFiles(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving cards from files: {Files}", string.Join(", ", filePaths));

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
                
                if (content == null)
                {
                    _logger.LogWarning("Skipping {FilePath} as it has no content", filePath);
                    continue;
                }
                
                _logger.LogDebug("Processing file: {FilePath}, Content length: {Length}", filePath, content.Length);

                // Parse the content
                var document = await _fileParser.ParseContentAsync(filePath, content, fileInfo.LastWriteTimeUtc);
                _logger.LogDebug("Parsed document from {FilePath} with {LineCount} lines", filePath, document.Content?.Split('\n').Length ?? 0);

                // Skip documents without tags
                if (document.Tags.NestedTags.Count == 0)
                {
                    _logger.LogDebug("Skipping {FilePath} as it has no tags", filePath);
                    continue;
                }

                var cards = _cardExtractor.ExtractCards(document).ToList();
                _logger.LogDebug("Extracted {Count} cards from {FilePath}", cards.Count, filePath);
                
                // Log each extracted card
                foreach (var card in cards)
                {
                    if (card is ParsedQuestionAnswerCard qaCard)
                    {
                        _logger.LogDebug("Card - Q: {Question}, A: {Answer}", qaCard.Question, qaCard.Answer);
                    }
                }
                
                allCards.AddRange(cards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process file {FilePath}", filePath);
                // Skip files that can't be parsed
                continue;
            }
        }

        _logger.LogInformation("Total cards extracted: {Count}", allCards.Count);
        var decks = _deckInferencer.InferDecks(allCards).ToList();
        _logger.LogInformation("Inferred {DeckCount} decks", decks.Count);
        
        var domainDecks = ConvertToDomainDecks(decks).ToList();
        _logger.LogInformation("Converted to {DomainDeckCount} domain decks", domainDecks.Count);
        
        foreach (var deck in domainDecks)
        {
            _logger.LogDebug("Deck {DeckId} has {CardCount} cards", deck.DeckId, deck.Cards?.Count ?? 0);
        }
        
        return domainDecks;
    }

    /// <summary>
    /// Retrieves all flashcards from the specified directories
    /// </summary>
    public async Task<IEnumerable<Deck>> GetCardsFromDirectories(IEnumerable<string> directories, CancellationToken cancellationToken = default)
    {
        if (directories == null)
        {
            throw new ArgumentNullException(nameof(directories));
        }

        _logger.LogInformation("Retrieving cards from directories: {Directories}", string.Join(", ", directories));

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