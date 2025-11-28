namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Extension methods for collections
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Gets distinct cards by ID
    /// </summary>
    /// <param name="cards">The cards collection</param>
    /// <returns>Distinct cards</returns>
    public static IEnumerable<Card> DistinctById(this IEnumerable<Card> cards)
    {
        return cards.GroupBy(c => c.Id).Select(g => g.First());
    }
}