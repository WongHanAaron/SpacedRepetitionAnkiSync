namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to move a card to a different deck in Anki.
/// </summary>
public class MoveCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the ID of the card to move.
    /// </summary>
    public long CardId { get; }

    /// <summary>
    /// Gets the ID of the target deck.
    /// </summary>
    public DeckId TargetDeckId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveCardInstruction"/> class.
    /// </summary>
    /// <param name="cardId">The ID of the card to move.</param>
    /// <param name="targetDeckId">The ID of the target deck.</param>
    public MoveCardInstruction(long cardId, DeckId targetDeckId)
    {
        if (cardId <= 0)
        {
            throw new ArgumentException("Card ID must be greater than 0.", nameof(cardId));
        }

        CardId = cardId;
        TargetDeckId = targetDeckId ?? throw new ArgumentNullException(nameof(targetDeckId));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.MoveCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{CardId}:{TargetDeckId}";
}