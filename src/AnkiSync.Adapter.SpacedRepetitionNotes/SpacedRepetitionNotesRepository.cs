using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Implementation of ICardSourceRepository for Spaced Repetition Notes
/// </summary>
public class SpacedRepetitionNotesRepository : ICardSourceRepository
{
    private readonly IFileParser _fileParser;
    private readonly ICardExtractor _cardExtractor;
    private readonly IDeckInferencer _deckInferencer;

    /// <summary>
    /// Initializes a new instance of SpacedRepetitionNotesRepository
    /// </summary>
    public SpacedRepetitionNotesRepository(
        IFileParser fileParser,
        ICardExtractor cardExtractor,
        IDeckInferencer deckInferencer)
    {
        _fileParser = fileParser;
        _cardExtractor = cardExtractor;
        _deckInferencer = deckInferencer;
    }

    /// <summary>
    /// Retrieves all flashcards from the specified file paths
    /// </summary>
    public async Task<IEnumerable<Deck>> GetCardsFromFiles(IEnumerable<string> filePaths, CancellationToken cancellationToken = default)
    {
        var allCards = new List<ParsedCard>();

        foreach (var filePath in filePaths)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var document = await _fileParser.ParseFileAsync(filePath);
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
    /// Retrieves all flashcards from files in the specified directories
    /// </summary>
    public async Task<IEnumerable<Deck>> GetCardsFromDirectories(IEnumerable<string> directories, CancellationToken cancellationToken = default)
    {
        var filePaths = new List<string>();

        foreach (var directory in directories)
        {
            if (Directory.Exists(directory))
            {
                // Find all markdown files
                var markdownFiles = Directory.GetFiles(directory, "*.md", SearchOption.AllDirectories);
                filePaths.AddRange(markdownFiles);
            }
        }

        return await GetCardsFromFiles(filePaths, cancellationToken);
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
                DeckId = DeckId.FromPath(parsedDeck.DeckPath.Split("::")),
                Cards = parsedDeck.Cards.Select(ConvertToDomainCard).ToList()
            };
            yield return deck;
        }
    }

    private Card ConvertToDomainCard(ParsedCard parsedCard)
    {
        // Generate a unique ID for the card
        var cardId = Guid.NewGuid().ToString();

        // Determine card type and create appropriate domain card
        if (parsedCard.CardType == "Cloze")
        {
            return new ClozeCard
            {
                Id = cardId,
                DateModified = DateTimeOffset.UtcNow,
                Text = parsedCard.Front,
                Answers = parsedCard.Answers
                // Tags are not part of the domain model yet
            };
        }
        else
        {
            return new QuestionAnswerCard
            {
                Id = cardId,
                DateModified = DateTimeOffset.UtcNow,
                Question = parsedCard.Front,
                Answer = parsedCard.Back
                // Tags are not part of the domain model yet
            };
        }
    }
}