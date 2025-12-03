namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to delete a card from Anki.
/// </summary>
public class DeleteCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the ID of the card to delete.
    /// </summary>
    public Card Card { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCardInstruction"/> class.
    /// </summary>
    /// <param name="cardId">The ID of the card to delete.</param>
    public DeleteCardInstruction(Card card)
    {
        Card = card ?? throw new ArgumentNullException(nameof(card));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.DeleteCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{Card}";

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}