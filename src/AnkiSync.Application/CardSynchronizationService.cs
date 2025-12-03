using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AnkiSync.Application;

/// <summary>
/// Service for synchronizing cards between source repository and deck repository
/// </summary>
public class CardSynchronizationService
{
    private readonly ICardSourceRepository _cardSourceRepository;
    private readonly IDeckRepository _deckRepository;
    private readonly ICardEqualityChecker _cardEqualityChecker;
    private readonly IDeckIdEqualityChecker _deckIdEqualityChecker;
    private readonly ILogger<CardSynchronizationService> _logger;

    /// <summary>
    /// Initializes a new instance of CardSynchronizationService
    /// </summary>
    public CardSynchronizationService(
        ICardSourceRepository cardSourceRepository,
        IDeckRepository deckRepository,
        ICardEqualityChecker cardEqualityChecker,
        IDeckIdEqualityChecker deckIdEqualityChecker,
        ILogger<CardSynchronizationService> logger)
    {
        _cardSourceRepository = cardSourceRepository ?? throw new ArgumentNullException(nameof(cardSourceRepository));
        _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
        _cardEqualityChecker = cardEqualityChecker ?? throw new ArgumentNullException(nameof(cardEqualityChecker));
        _deckIdEqualityChecker = deckIdEqualityChecker ?? throw new ArgumentNullException(nameof(deckIdEqualityChecker));
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

        // Get all existing Anki decks
        var existingDecks = await _deckRepository.GetAllDecksAsync(cancellationToken);

        _logger.LogDebug("Retrieved {Count} existing Anki decks", existingDecks.Count());

        // Accumulate synchronization instructions
        var instructions = await AccumulateInstructionsAsync(sourceDecks, existingDecks, cancellationToken);

        _logger.LogDebug("Accumulated {Count} synchronization instructions", instructions.Count);

        await ExecuteInstructionsAsync(instructions, cancellationToken);
    }

    /// <summary>
    /// Accumulates synchronization instructions by comparing source decks with existing Anki decks
    /// </summary>
    /// <param name="sourceDecks">The decks from source files</param>
    /// <param name="existingAnkiDecks">The existing decks in Anki</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of synchronization instructions to execute</returns>
    internal Task<IReadOnlyList<SynchronizationInstruction>> AccumulateInstructionsAsync(
        IEnumerable<Deck> sourceDecks,
        IEnumerable<Deck> existingAnkiDecks,
        CancellationToken cancellationToken = default)
    {
        if (sourceDecks == null)
        {
            throw new ArgumentNullException(nameof(sourceDecks));
        }
        if (existingAnkiDecks == null)
        {
            throw new ArgumentNullException(nameof(existingAnkiDecks));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var instructions = new List<SynchronizationInstruction>();

        // Convert to lists for easier manipulation
        var sourceDeckList = sourceDecks.ToList();
        var existingDeckList = existingAnkiDecks.ToList();

        // Collect all existing cards across all decks for global duplicate checking
        var allExistingCards = existingDeckList.SelectMany(d => d.Cards.Select(c =>((DeckId DeckId, Card Card))(d.DeckId, c)) ?? new List<(DeckId DeckId, Card Card)>()).ToList();

        // Find decks to delete (exist in Anki but not in source)
        // Exclude Default deck and decks with children
        var decksToDelete = existingDeckList.Where(existingDeck =>
            !sourceDeckList.Any(sourceDeck => _deckIdEqualityChecker.AreEqual(sourceDeck.DeckId, existingDeck.DeckId)) &&
            !IsDefaultDeck(existingDeck.DeckId) &&
            !HasChildDecks(existingDeck.DeckId, existingDeckList));

        foreach (var deckToDelete in decksToDelete)
        {
            instructions.Add(new DeleteDeckInstruction(deckToDelete.DeckId));
        }
        
        var sourceCards = sourceDeckList.SelectMany(d => d.Cards).ToList();
        // Find cards to delete (exist in Anki but not in source for this deck)
        var cardsToDelete = allExistingCards.Where(existingCard =>
            !sourceCards.Any(sourceCard => _cardEqualityChecker.AreEqual(sourceCard, existingCard.Card)));

        foreach (var cardToDelete in cardsToDelete)
        {
            instructions.Add(new DeleteCardInstruction(cardToDelete.Card));
        }

        // Process source decks
        foreach (var sourceDeck in sourceDeckList)
        {
            var existingDeck = existingDeckList.FirstOrDefault(d => _deckIdEqualityChecker.AreEqual(d.DeckId, sourceDeck.DeckId));

            if (existingDeck == null)
            {
                // New deck - create it and all its cards
                instructions.Add(new CreateDeckInstruction(sourceDeck.DeckId));
                foreach (var card in sourceDeck.Cards ?? new List<Card>())
                {
                    instructions.Add(new CreateCardInstruction(sourceDeck.DeckId, card));
                }
            }
            else
            {
                // Existing deck - compare cards
                AccumulateCardInstructions(sourceDeck, existingDeck, allExistingCards, instructions, cancellationToken);
            }
        }

        // Add final sync instruction
        instructions.Add(new SyncWithAnkiInstruction());

        return Task.FromResult<IReadOnlyList<SynchronizationInstruction>>(instructions);
    }

    /// <summary>
    /// Accumulates card-level synchronization instructions for an existing deck
    /// </summary>
    private void AccumulateCardInstructions(
        Deck sourceDeck,
        Deck existingDeck,
        List<(DeckId DeckId, Card Card)> allExistingCards,
        List<SynchronizationInstruction> instructions,
        CancellationToken cancellationToken)
    {
        var sourceCards = sourceDeck.Cards ?? new List<Card>();
        var existingCards = existingDeck.Cards ?? new List<Card>();

        // Process source cards
        foreach (var sourceCard in sourceCards)
        {
            // Check for matching card across all existing decks
            var matchingExistingCard = allExistingCards.FirstOrDefault(c => _cardEqualityChecker.AreEqual(c.Card, sourceCard));

            if (matchingExistingCard.Card == null) 
            {
                // New card - create it
                instructions.Add(new CreateCardInstruction(sourceDeck.DeckId, sourceCard));
            }
            else
            {
                // Card exists somewhere - check if it's in the correct deck
                var isInCurrentDeck = _deckIdEqualityChecker.AreEqual(sourceDeck.DeckId, matchingExistingCard.DeckId);

                if (isInCurrentDeck)
                {
                    // Card is already in the correct deck - check if it needs updating
                    var existingCardInDeck = existingCards.First(c => _cardEqualityChecker.AreEqual(c, sourceCard));
                    if (HasCardChanged(existingCardInDeck, sourceCard))
                    {
                        instructions.Add(new UpdateCardInstruction(existingCardInDeck, sourceCard));
                    }
                    // If no change, do nothing
                }
                else
                {
                    // Card exists in a different deck - check if it needs updating and move it to this deck
                    // Check if the card content has changed
                    if (HasCardChanged(matchingExistingCard.Card, sourceCard))
                    {
                        instructions.Add(new UpdateCardInstruction(matchingExistingCard.Card, sourceCard));
                    }
                    // Move the card to the current deck
                    instructions.Add(new MoveCardInstruction(matchingExistingCard.Card, sourceDeck.DeckId));
                }
            }
        }
    }

    /// <summary>
    /// Executes the accumulated synchronization instructions
    /// </summary>
    private async Task ExecuteInstructionsAsync(
        IReadOnlyList<SynchronizationInstruction> instructions,
        CancellationToken cancellationToken = default)
    {
        await _deckRepository.ExecuteInstructionsAsync(instructions, cancellationToken);
    }

    /// <summary>
    /// Checks if the given deck ID represents the Default deck
    /// </summary>
    private static bool IsDefaultDeck(DeckId deckId)
    {
        return deckId.Name == "Default" && (deckId.Parents == null || deckId.Parents.Count == 0);
    }

    /// <summary>
    /// Checks if the given deck ID has any child decks in the provided deck list
    /// </summary>
    private static bool HasChildDecks(DeckId deckId, IEnumerable<Deck> allDecks)
    {
        var parentPath = deckId.Parents.Concat(new[] { deckId.Name }).ToList();
        
        return allDecks.Any(deck => 
            deck.DeckId.Parents != null && 
            deck.DeckId.Parents.Count >= parentPath.Count && 
            parentPath.SequenceEqual(deck.DeckId.Parents.Take(parentPath.Count)));
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
        return !AreCardsEqual(existingCard, sourceCard);
    }
}