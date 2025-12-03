namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to move a card to a different deck in Anki.
/// </summary>
public class MoveCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the card to move.
    /// </summary>
    public Card Card { get; }

    /// <summary>
    /// Gets the ID of the target deck.
    /// </summary>
    public DeckId TargetDeckId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveCardInstruction"/> class.
    /// </summary>
    /// <param name="card">The card to move.</param>
    /// <param name="targetDeckId">The ID of the target deck.</param>
    public MoveCardInstruction(Card card, DeckId targetDeckId)
    {
        Card = card ?? throw new ArgumentNullException(nameof(card));

        TargetDeckId = targetDeckId ?? throw new ArgumentNullException(nameof(targetDeckId));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.MoveCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{System.Text.Json.JsonSerializer.Serialize(Card)}:{TargetDeckId}";

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}