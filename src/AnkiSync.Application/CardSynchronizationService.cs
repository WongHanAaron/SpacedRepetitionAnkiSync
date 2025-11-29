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

        // Ensure source deck cards are initialized
        if (sourceDeck.Cards == null)
        {
            sourceDeck.Cards = new List<Card>();
        }

        // Try to get existing deck from repository
        var existingDeck = await _deckRepository.GetDeck(sourceDeck.DeckId, cancellationToken);
        
        if (existingDeck == null)
        {
            _logger.LogDebug("Creating new deck {DeckId}", sourceDeck.DeckId);
            // Create a new deck with the source deck's cards
            var newDeck = new Deck
            {
                DeckId = sourceDeck.DeckId,
                Cards = new List<Card>(sourceDeck.Cards)
            };
            
            await _deckRepository.UpsertDeck(newDeck, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Updating existing deck {DeckId}", sourceDeck.DeckId);
            // Merge source deck with existing deck
            var mergedDeck = MergeDecks(existingDeck, sourceDeck);
            
            // Update the deck in the repository
            await _deckRepository.UpsertDeck(mergedDeck, cancellationToken);
        }
    }

    private Deck MergeDecks(Deck existingDeck, Deck sourceDeck)
    {
        // If existingDeck is null or has no cards, just return a copy of sourceDeck
        if (existingDeck == null || existingDeck.Cards == null || !existingDeck.Cards.Any())
        {
            return new Deck
            {
                Id = sourceDeck.Id,
                DeckId = sourceDeck.DeckId,
                Cards = new List<Card>(sourceDeck.Cards ?? new List<Card>())
            };
        }

        // If sourceDeck has no cards, return a copy of existingDeck
        if (sourceDeck.Cards == null || !sourceDeck.Cards.Any())
        {
            return new Deck
            {
                Id = existingDeck.Id,
                DeckId = existingDeck.DeckId,
                Cards = new List<Card>(existingDeck.Cards)
            };
        }

        // Create a new deck with the same ID but merged cards
        var mergedCards = new List<Card>();
        
        // Add all source cards (these are the new/updated cards)
        foreach (var sourceCard in sourceDeck.Cards)
        {
            // For new decks, there won't be any existing cards to match
            if (existingDeck.Cards == null || !existingDeck.Cards.Any())
            {
                mergedCards.Add(sourceCard);
                continue;
            }

            var existingCard = FindMatchingCard(existingDeck.Cards, sourceCard);
            
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
                mergedCards.Add(updatedCard);
            }
            else
            {
                // Card not changed, keep existing
                mergedCards.Add(existingCard);
            }
        }

        // Add any existing cards that weren't in the source (not touched by this update)
        if (existingDeck.Cards != null)
        {
            foreach (var existingCard in existingDeck.Cards)
            {
                if (!mergedCards.Any(c => c.Id == existingCard.Id))
                {
                    mergedCards.Add(existingCard);
                }
            }
        }

        return new Deck
        {
            Id = existingDeck.Id,
            DeckId = sourceDeck.DeckId,
            Cards = mergedCards
        };
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