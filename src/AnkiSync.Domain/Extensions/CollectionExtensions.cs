namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Extension methods for collections
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Gets distinct cards by content (question for QA cards, text for cloze cards)
    /// </summary>
    /// <param name="cards">The cards collection</param>
    /// <returns>Distinct cards</returns>
    public static IEnumerable<Card> DistinctByContent(this IEnumerable<Card> cards)
    {
        return cards.GroupBy(c => GetCardContentKey(c)).Select(g => g.First());
    }

    private static string GetCardContentKey(Card card)
    {
        return card switch
        {
            QuestionAnswerCard qa => $"QA:{qa.Question}",
            ClozeCard cloze => $"Cloze:{cloze.Text}",
            _ => card.GetHashCode().ToString()
        };
    }
}