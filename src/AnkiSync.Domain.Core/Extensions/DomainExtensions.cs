using AnkiSync.Domain.Core.Exceptions;
using System.Globalization;

namespace AnkiSync.Domain.Core.Extensions;

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

/// <summary>
/// Extension methods for string operations related to Anki
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts an Obsidian tag to an Anki deck name
    /// </summary>
    /// <param name="tag">The Obsidian tag (e.g., "algorithms/datastructures")</param>
    /// <returns>Anki deck name (e.g., "Algorithms::Datastructures")</returns>
    public static string ToAnkiDeckName(this string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return "Default";
        }

        // Remove the # prefix if present
        var cleanTag = tag.TrimStart('#');

        // Split by / and capitalize each part
        var parts = cleanTag.Split('/')
            .Select(part => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(part.ToLower()))
            .ToArray();

        // Join with :: for Anki hierarchy
        return string.Join("::", parts);
    }

    /// <summary>
    /// Checks if a string is a valid Obsidian tag
    /// </summary>
    /// <param name="tag">The potential tag</param>
    /// <returns>True if valid tag format</returns>
    public static bool IsValidObsidianTag(this string tag)
    {
        return !string.IsNullOrWhiteSpace(tag) &&
               tag.StartsWith('#') &&
               tag.Length > 1 &&
               !tag.Contains(' ') &&
               tag.All(c => char.IsLetterOrDigit(c) || c == '#' || c == '/' || c == '-' || c == '_');
    }
}

/// <summary>
/// Extension methods for collections
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Gets distinct flashcards by ID
    /// </summary>
    /// <param name="flashcards">The flashcards collection</param>
    /// <returns>Distinct flashcards</returns>
    public static IEnumerable<Flashcard> DistinctById(this IEnumerable<Flashcard> flashcards)
    {
        return flashcards.GroupBy(f => f.Id).Select(g => g.First());
    }

    /// <summary>
    /// Gets flashcards that have been modified since the given time
    /// </summary>
    /// <param name="flashcards">The flashcards collection</param>
    /// <param name="since">The cutoff time</param>
    /// <returns>Modified flashcards</returns>
    public static IEnumerable<Flashcard> ModifiedSince(this IEnumerable<Flashcard> flashcards, DateTime since)
    {
        return flashcards.Where(f => f.LastModified > since);
    }
}