using AnkiSync.Domain.Exceptions;

namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Extension methods for Flashcard validation and operations
/// </summary>
public static class FlashcardExtensions
{
    /// <summary>
    /// Validates that a flashcard has required fields
    /// </summary>
    /// <param name="flashcard">The flashcard to validate</param>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    public static void Validate(this Flashcard flashcard)
    {
        if (string.IsNullOrWhiteSpace(flashcard.Question))
        {
            throw new ValidationException("Flashcard question cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(flashcard.Answer))
        {
            throw new ValidationException("Flashcard answer cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(flashcard.SourceFile))
        {
            throw new ValidationException("Flashcard source file cannot be empty");
        }

        if (flashcard.Tags.Count == 0)
        {
            throw new ValidationException("Flashcard must have at least one tag for deck inference");
        }
    }

    /// <summary>
    /// Gets the first tag from the flashcard (used for deck inference)
    /// </summary>
    /// <param name="flashcard">The flashcard</param>
    /// <returns>The first tag, or empty string if no tags</returns>
    public static string GetFirstTag(this Flashcard flashcard)
    {
        return flashcard.Tags.FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Checks if the flashcard has been synced to Anki
    /// </summary>
    /// <param name="flashcard">The flashcard</param>
    /// <returns>True if synced</returns>
    public static bool IsSynced(this Flashcard flashcard)
    {
        return !string.IsNullOrWhiteSpace(flashcard.AnkiNoteId);
    }
}