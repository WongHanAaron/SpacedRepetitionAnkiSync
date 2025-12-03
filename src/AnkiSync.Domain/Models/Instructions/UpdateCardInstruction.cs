namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to update an existing card in Anki.
/// </summary>
public class UpdateCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the ID of the card to update.
    /// </summary>
    public long CardId { get; }

    /// <summary>
    /// Gets the updated card data.
    /// </summary>
    public Card Card { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCardInstruction"/> class.
    /// </summary>
    /// <param name="cardId">The ID of the card to update.</param>
    /// <param name="card">The updated card data.</param>
    public UpdateCardInstruction(long cardId, Card card)
    {
        if (cardId <= 0)
        {
            throw new ArgumentException("Card ID must be greater than 0.", nameof(cardId));
        }

        CardId = cardId;
        Card = card ?? throw new ArgumentNullException(nameof(card));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.UpdateCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{CardId}";

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}