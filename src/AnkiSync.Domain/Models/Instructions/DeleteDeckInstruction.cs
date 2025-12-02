namespace AnkiSync.Domain.Models;

/// <summary>
/// Instruction to delete a deck from Anki.
/// </summary>
public class DeleteDeckInstruction : SynchronizationInstruction
{
    /// <summary>
    /// Gets the ID of the deck to delete.
    /// </summary>
    public DeckId DeckId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDeckInstruction"/> class.
    /// </summary>
    /// <param name="deckId">The ID of the deck to delete.</param>
    public DeleteDeckInstruction(DeckId deckId)
    {
        DeckId = deckId ?? throw new ArgumentNullException(nameof(deckId));
    }

    /// <inheritdoc />
    public override SynchronizationInstructionType InstructionType => SynchronizationInstructionType.DeleteDeck;

    /// <inheritdoc />
    public override string GetUniqueKey() => $"{InstructionType}:{DeckId}";
}