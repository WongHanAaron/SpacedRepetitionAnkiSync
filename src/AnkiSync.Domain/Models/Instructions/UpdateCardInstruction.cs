namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to update an existing card in Anki.
/// </summary>
public class UpdateCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the existing card (from Anki) that will be updated.
    /// </summary>
    public Card ExistingCard { get; }

    /// <summary>
    /// Gets the updated card data (from source).
    /// </summary>
    public Card Card { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCardInstruction"/> class.
    /// </summary>
    /// <param name="existingCard">The existing card from Anki to update.</param>
    /// <param name="card">The updated card data from source.</param>
    public UpdateCardInstruction(Card existingCard, Card card)
    {
        ExistingCard = existingCard ?? throw new ArgumentNullException(nameof(existingCard));
        Card = card ?? throw new ArgumentNullException(nameof(card));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.UpdateCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{System.Text.Json.JsonSerializer.Serialize(ExistingCard)}";

    /// <inheritdoc />
    public override string ToString() => $"{InstructionType}: {System.Text.Json.JsonSerializer.Serialize(this)}";
}