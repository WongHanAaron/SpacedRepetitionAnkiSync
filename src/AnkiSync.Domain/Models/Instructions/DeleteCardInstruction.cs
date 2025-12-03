namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to delete a card from Anki.
/// </summary>
public class DeleteCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the ID of the card to delete.
    /// </summary>
    public long CardId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCardInstruction"/> class.
    /// </summary>
    /// <param name="cardId">The ID of the card to delete.</param>
    public DeleteCardInstruction(long cardId)
    {
        if (cardId <= 0)
        {
            throw new ArgumentException("Card ID must be greater than 0.", nameof(cardId));
        }

        CardId = cardId;
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.DeleteCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{CardId}";

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}