using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AnkiSync.Application;

/// <summary>
/// Service for synchronizing cards between source repository and deck repository
/// </summary>
public class CardSynchronizationService
{
    private readonly ICardSourceRepository _cardSourceRepository;
    private readonly IDeckRepository _deckRepository;
    private readonly ILogger<CardSynchronizationService> _logger;

    /// <summary>
    /// Initializes a new instance of CardSynchronizationService
    /// </summary>
    public CardSynchronizationService(
        ICardSourceRepository cardSourceRepository,
        IDeckRepository deckRepository,
        ILogger<CardSynchronizationService> logger)
    {
        _cardSourceRepository = cardSourceRepository ?? throw new ArgumentNullException(nameof(cardSourceRepository));
        _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Synchronizes cards from the source repository to the deck repository
    /// </summary>
    /// <param name="directories">The directories to scan for card sources</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SynchronizeCardsAsync(IEnumerable<string> directories, CancellationToken cancellationToken = default)
    {
        if (directories == null)
        {
            throw new ArgumentNullException(nameof(directories));
        }
        _logger.LogInformation("Starting card synchronization for directories: {Directories}", string.Join(", ", directories));

        // Get all decks from the source repository
        var sourceDecks = await _cardSourceRepository.GetCardsFromDirectories(directories, cancellationToken);

        _logger.LogDebug("Retrieved {Count} source decks", sourceDecks.Count());

        foreach (var sourceDeck in sourceDecks)
        {
            await SynchronizeDeckAsync(sourceDeck, cancellationToken);
        }
    }

    private async Task SynchronizeDeckAsync(Deck sourceDeck, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Synchronizing deck {DeckId}", sourceDeck.DeckId);

        try
        {
            // Try to get existing deck from repository
            var existingDeck = await _deckRepository.GetDeck(sourceDeck.DeckId, cancellationToken);
            
            // Merge source deck with existing deck
            var mergedDeck = MergeDecks(existingDeck, sourceDeck);
            
            // Update the deck in the repository
            await _deckRepository.UpsertDeck(mergedDeck, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize deck {DeckId}", sourceDeck.DeckId);

            // Deck doesn't exist, create it
            await _deckRepository.UpsertDeck(sourceDeck, cancellationToken);
        }
    }

    private Deck MergeDecks(Deck existingDeck, Deck sourceDeck)
    {
        // Create a new deck with the same ID but merged cards
        var mergedDeck = new Deck
        {
            Id = existingDeck.Id, // Keep existing ID
            DeckId = sourceDeck.DeckId, // Use source deck ID
            Cards = MergeCards(existingDeck.Cards, sourceDeck.Cards)
        };

        return mergedDeck;
    }

    private List<Card> MergeCards(List<Card> existingCards, List<Card> sourceCards)
    {
        var mergedCards = new List<Card>(existingCards); // Start with existing cards

        foreach (var sourceCard in sourceCards)
        {
            var existingCard = FindMatchingCard(existingCards, sourceCard);
            
            if (existingCard == null)
            {
                // New card, add it
                mergedCards.Add(sourceCard);
            }
            else if (HasCardChanged(existingCard, sourceCard))
            {
                // Card changed, update it but preserve the existing ID
                Card updatedCard = sourceCard switch
                {
                    QuestionAnswerCard qa => new QuestionAnswerCard
                    {
                        Id = existingCard.Id,
                        Question = qa.Question,
                        Answer = qa.Answer,
                        DateModified = qa.DateModified
                    },
                    ClozeCard cloze => new ClozeCard
                    {
                        Id = existingCard.Id,
                        Text = cloze.Text,
                        Answers = cloze.Answers,
                        DateModified = cloze.DateModified
                    },
                    _ => sourceCard
                };
                var index = mergedCards.IndexOf(existingCard);
                mergedCards[index] = updatedCard;
            }
            // If card exists and hasn't changed, keep existing
        }

        return mergedCards;
    }

    private Card? FindMatchingCard(List<Card> cards, Card targetCard)
    {
        // Match cards that should be updated (same question for QA, same text for cloze)
        return cards.FirstOrDefault(c => CardsMatchForUpdate(c, targetCard));
    }

    private bool CardsMatchForUpdate(Card card1, Card card2)
    {
        if (card1.Type != card2.Type)
            return false;

        return card1 switch
        {
            QuestionAnswerCard qa1 when card2 is QuestionAnswerCard qa2 =>
                qa1.Question == qa2.Question,
            ClozeCard cloze1 when card2 is ClozeCard cloze2 =>
                cloze1.Text == cloze2.Text,
            _ => false
        };
    }

    private bool AreCardsEqual(Card card1, Card card2)
    {
        if (card1.Type != card2.Type)
            return false;

        return card1 switch
        {
            QuestionAnswerCard qa1 when card2 is QuestionAnswerCard qa2 =>
                qa1.Question == qa2.Question && qa1.Answer == qa2.Answer,
            ClozeCard cloze1 when card2 is ClozeCard cloze2 =>
                cloze1.Text == cloze2.Text && 
                cloze1.Answers.Count == cloze2.Answers.Count &&
                cloze1.Answers.All(kvp => cloze2.Answers.TryGetValue(kvp.Key, out var value) && value == kvp.Value),
            _ => false
        };
    }

    private bool HasCardChanged(Card existingCard, Card sourceCard)
    {
        // Compare DateModified or content
        return sourceCard.DateModified > existingCard.DateModified ||
               !AreCardsEqual(existingCard, sourceCard);
    }
}