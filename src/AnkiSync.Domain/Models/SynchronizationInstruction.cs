namespace AnkiSync.Domain.Models;

/// <summary>
/// Enumeration of synchronization instruction types.
/// </summary>
public enum SynchronizationInstructionType
{
    CreateDeck,
    DeleteDeck,
    CreateCard,
    UpdateCard,
    DeleteCard,
    MoveCard,
    SyncWithAnki
}

/// <summary>
/// Base class for all synchronization instructions.
/// Instructions represent operations that need to be performed to synchronize Anki with source decks.
/// </summary>
public abstract class SynchronizationInstruction
{
    /// <summary>
    /// Gets the type of synchronization instruction.
    /// </summary>
    public abstract SynchronizationInstructionType InstructionType { get; }

    /// <summary>
    /// Gets a unique identifier for this instruction to support deduplication.
    /// </summary>
    public virtual string GetUniqueKey() => InstructionType.ToString();
}