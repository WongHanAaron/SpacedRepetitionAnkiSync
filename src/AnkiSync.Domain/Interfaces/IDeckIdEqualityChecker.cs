namespace AnkiSync.Domain.Interfaces;

using AnkiSync.Domain.Models;

/// <summary>
/// Interface for checking equality between two DeckId instances.
/// </summary>
public interface IDeckIdEqualityChecker
{
    /// <summary>
    /// Determines whether two DeckId instances are equal according to the implementation's rules.
    /// </summary>
    bool AreEqual(DeckId? a, DeckId? b);
}