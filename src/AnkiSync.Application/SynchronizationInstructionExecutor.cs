using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AnkiSync.Application;

/// <summary>
/// Implementation of ISynchronizationInstructionExecutor that executes synchronization instructions.
/// </summary>
public class SynchronizationInstructionExecutor : ISynchronizationInstructionExecutor
{
    private readonly IDeckRepository _deckRepository;
    private readonly ILogger<SynchronizationInstructionExecutor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizationInstructionExecutor"/> class.
    /// </summary>
    /// <param name="deckRepository">The deck repository.</param>
    /// <param name="logger">The logger.</param>
    public SynchronizationInstructionExecutor(
        IDeckRepository deckRepository,
        ILogger<SynchronizationInstructionExecutor> logger)
    {
        _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task ExecuteInstructionsAsync(IEnumerable<SynchronizationInstruction> instructions, CancellationToken cancellationToken = default)
    {
        if (instructions == null)
        {
            throw new ArgumentNullException(nameof(instructions));
        }

        var instructionList = instructions.ToList();
        _logger.LogInformation("Executing {Count} synchronization instructions", instructionList.Count);

        foreach (var instruction in instructionList)
        {
            try
            {
                await ExecuteInstructionAsync(instruction, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing instruction {InstructionType}: {UniqueKey}",
                    instruction.InstructionType, instruction.GetUniqueKey());
                throw;
            }
        }

        _logger.LogInformation("Successfully executed all synchronization instructions");
    }

    private async Task ExecuteInstructionAsync(SynchronizationInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing instruction {InstructionType}: {UniqueKey}",
            instruction.InstructionType, instruction.GetUniqueKey());

        switch (instruction)
        {
            case CreateDeckInstruction createDeck:
                await ExecuteCreateDeckAsync(createDeck, cancellationToken);
                break;
            case DeleteDeckInstruction deleteDeck:
                await ExecuteDeleteDeckAsync(deleteDeck, cancellationToken);
                break;
            case CreateCardInstruction createCard:
                await ExecuteCreateCardAsync(createCard, cancellationToken);
                break;
            case UpdateCardInstruction updateCard:
                await ExecuteUpdateCardAsync(updateCard, cancellationToken);
                break;
            case DeleteCardInstruction deleteCard:
                await ExecuteDeleteCardAsync(deleteCard, cancellationToken);
                break;
            case MoveCardInstruction moveCard:
                await ExecuteMoveCardAsync(moveCard, cancellationToken);
                break;
            case SyncWithAnkiInstruction syncWithAnki:
                await ExecuteSyncWithAnkiAsync(syncWithAnki, cancellationToken);
                break;
            default:
                throw new NotSupportedException($"Instruction type {instruction.InstructionType} is not supported");
        }
    }

    private async Task ExecuteCreateDeckAsync(CreateDeckInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating deck {DeckId}", instruction.DeckId);

        // Create an empty deck
        var deck = new Deck
        {
            DeckId = instruction.DeckId,
            Cards = new List<Card>()
        };

        await _deckRepository.UpsertDeck(deck, cancellationToken);
    }

    private async Task ExecuteDeleteDeckAsync(DeleteDeckInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting deck {DeckId}", instruction.DeckId);
        await _deckRepository.DeleteDeckAsync(instruction.DeckId, cancellationToken);
    }

    private async Task ExecuteCreateCardAsync(CreateCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating card in deck {DeckId}", instruction.DeckId);

        var deck = await _deckRepository.GetDeck(instruction.DeckId, cancellationToken) ?? new Deck
        {
            DeckId = instruction.DeckId,
            Cards = new List<Card>()
        };

        // Add the new card to the deck
        deck.Cards.Add(instruction.Card);

        await _deckRepository.UpsertDeck(deck, cancellationToken);
    }

    private async Task ExecuteUpdateCardAsync(UpdateCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating card {CardId}", instruction.CardId);

        // Find which deck contains this card
        var allDeckIds = await _deckRepository.GetAllDeckIdsAsync(cancellationToken);
        Deck? targetDeck = null;

        foreach (var deckId in allDeckIds)
        {
            var deck = await _deckRepository.GetDeck(deckId, cancellationToken);
            if (deck?.Cards?.Any(c => c.Id == instruction.CardId) == true)
            {
                targetDeck = deck;
                break;
            }
        }

        if (targetDeck == null)
        {
            throw new InvalidOperationException($"Card {instruction.CardId} not found in any deck");
        }

        // Update the card in the deck
        var cardIndex = targetDeck.Cards.FindIndex(c => c.Id == instruction.CardId);
        if (cardIndex >= 0)
        {
            targetDeck.Cards[cardIndex] = instruction.Card;
            await _deckRepository.UpsertDeck(targetDeck, cancellationToken);
        }
    }

    private async Task ExecuteDeleteCardAsync(DeleteCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting card {CardId}", instruction.CardId);

        // Find which deck contains this card
        var allDeckIds = await _deckRepository.GetAllDeckIdsAsync(cancellationToken);
        Deck? targetDeck = null;

        foreach (var deckId in allDeckIds)
        {
            var deck = await _deckRepository.GetDeck(deckId, cancellationToken);
            if (deck?.Cards?.Any(c => c.Id == instruction.CardId) == true)
            {
                targetDeck = deck;
                break;
            }
        }

        if (targetDeck == null)
        {
            _logger.LogWarning("Card {CardId} not found in any deck, skipping delete", instruction.CardId);
            return;
        }

        // Remove the card from the deck
        targetDeck.Cards.RemoveAll(c => c.Id == instruction.CardId);
        await _deckRepository.UpsertDeck(targetDeck, cancellationToken);
    }

    private async Task ExecuteMoveCardAsync(MoveCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Moving card {CardId} to deck {TargetDeckId}", instruction.CardId, instruction.TargetDeckId);

        // Find the source deck containing this card
        var allDeckIds = await _deckRepository.GetAllDeckIdsAsync(cancellationToken);
        Deck? sourceDeck = null;
        Card? cardToMove = null;

        foreach (var deckId in allDeckIds)
        {
            var deck = await _deckRepository.GetDeck(deckId, cancellationToken);
            var card = deck?.Cards?.FirstOrDefault(c => c.Id == instruction.CardId);
            if (card != null)
            {
                sourceDeck = deck;
                cardToMove = card;
                break;
            }
        }

        if (sourceDeck == null || cardToMove == null)
        {
            throw new InvalidOperationException($"Card {instruction.CardId} not found in any deck");
        }

        // Remove from source deck
        sourceDeck.Cards.RemoveAll(c => c.Id == instruction.CardId);
        await _deckRepository.UpsertDeck(sourceDeck, cancellationToken);

        // Add to target deck
        var targetDeck = await _deckRepository.GetDeck(instruction.TargetDeckId, cancellationToken) ?? new Deck
        {
            DeckId = instruction.TargetDeckId,
            Cards = new List<Card>()
        };

        targetDeck.Cards.Add(cardToMove);
        await _deckRepository.UpsertDeck(targetDeck, cancellationToken);
    }

    private async Task ExecuteSyncWithAnkiAsync(SyncWithAnkiInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Syncing with AnkiWeb");
        await _deckRepository.SyncWithAnkiWebAsync(cancellationToken);
    }
}