namespace AnkiSync.Domain.Extensions;

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