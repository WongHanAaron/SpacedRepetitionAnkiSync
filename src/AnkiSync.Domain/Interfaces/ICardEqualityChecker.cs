namespace AnkiSync.Domain.Interfaces;

/// <summary>
/// Interface for checking equality between cards.
/// </summary>
public interface ICardEqualityChecker
{
    /// <summary>
    /// Determines whether two cards are equal based on the implementation's criteria.
    /// </summary>
    /// <param name="source">The source card to compare.</param>
    /// <param name="target">The target card to compare.</param>
    /// <returns>True if the cards are considered equal, false otherwise.</returns>
    bool AreEqual(Card source, Card target);
}