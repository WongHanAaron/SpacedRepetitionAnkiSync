namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to create a new card in Anki.
/// </summary>
public class CreateCardInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the ID of the deck where the card should be created.
    /// </summary>
    public DeckId DeckId { get; }

    /// <summary>
    /// Gets the card to create.
    /// </summary>
    public Card Card { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCardInstruction"/> class.
    /// </summary>
    /// <param name="deckId">The ID of the deck where the card should be created.</param>
    /// <param name="card">The card to create.</param>
    public CreateCardInstruction(DeckId deckId, Card card)
    {
        DeckId = deckId ?? throw new ArgumentNullException(nameof(deckId));
        Card = card ?? throw new ArgumentNullException(nameof(card));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.CreateCard;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{DeckId}:{Card.GetHashCode()}";

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}