namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to create a new deck in Anki.
/// </summary>
public class CreateDeckInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the deck identifier for the deck to create.
    /// </summary>
    public DeckId DeckId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDeckInstruction"/> class.
    /// </summary>
    /// <param name="deckId">The deck identifier for the deck to create.</param>
    public CreateDeckInstruction(DeckId deckId)
    {
        DeckId = deckId ?? throw new ArgumentNullException(nameof(deckId));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.CreateDeck;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{DeckId}";
}