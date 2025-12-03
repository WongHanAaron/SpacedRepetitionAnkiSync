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

        // Execute instructions (to be implemented)
        await ExecuteInstructionsAsync(instructions, cancellationToken);

        // Sync with AnkiWeb after all instructions have been executed
        try
        {
            _logger.LogInformation("Syncing with AnkiWeb after synchronization");
            await _deckRepository.SyncWithAnkiWebAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing with AnkiWeb");
        }
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

        // Find decks to delete (exist in Anki but not in source)
        var decksToDelete = existingDeckList.Where(existingDeck =>
            !sourceDeckList.Any(sourceDeck => _deckIdEqualityChecker.AreEqual(sourceDeck.DeckId, existingDeck.DeckId)));

        foreach (var deckToDelete in decksToDelete)
        {
            instructions.Add(new DeleteDeckInstruction(deckToDelete.DeckId));
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
                AccumulateCardInstructions(sourceDeck, existingDeck, instructions, cancellationToken);
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
        List<SynchronizationInstruction> instructions,
        CancellationToken cancellationToken)
    {
        var sourceCards = sourceDeck.Cards ?? new List<Card>();
        var existingCards = existingDeck.Cards ?? new List<Card>();

        // Find cards to delete (exist in Anki but not in source)
        var cardsToDelete = existingCards.Where(existingCard =>
            !sourceCards.Any(sourceCard => _cardEqualityChecker.AreEqual(sourceCard, existingCard)));

        foreach (var cardToDelete in cardsToDelete)
        {
            if (cardToDelete.Id.HasValue)
            {
                instructions.Add(new DeleteCardInstruction(cardToDelete.Id.Value));
            }
        }

        // Process source cards
        foreach (var sourceCard in sourceCards)
        {
            var existingCard = existingCards.FirstOrDefault(c => _cardEqualityChecker.AreEqual(c, sourceCard));

            if (existingCard == null)
            {
                // New card
                instructions.Add(new CreateCardInstruction(sourceDeck.DeckId, sourceCard));
            }
            else if (HasCardChanged(existingCard, sourceCard))
            {
                // Card changed - update using existing card's ID
                if (existingCard.Id.HasValue)
                {
                    instructions.Add(new UpdateCardInstruction(existingCard.Id.Value, sourceCard));
                }
            }
            // If card exists and hasn't changed, do nothing
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