using AnkiSync.Domain.Exceptions;

namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Extension methods for ClozeCard validation and operations
/// </summary>
public static class ClozeCardExtensions
{
    /// <summary>
    /// Validates that a cloze card has required fields
    /// </summary>
    /// <param name="card">The card to validate</param>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    public static void Validate(this ClozeCard card)
    {
        if (string.IsNullOrWhiteSpace(card.Text))
        {
            throw new ValidationException("Cloze card text cannot be empty");
        }

        // Check for cloze deletions (e.g., {{c1::text}})
        if (!card.Text.Contains("{{") || !card.Text.Contains("}}"))
        {
            throw new ValidationException("Cloze card must contain cloze deletions in the format {{c1::text}}");
        }
    }
}