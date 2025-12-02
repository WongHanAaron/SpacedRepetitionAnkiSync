using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;

namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Implementation of ICardEqualityChecker that performs exact matching.
/// For QuestionAndAnswerCards, checks if the question is exactly the same.
/// For ClozeCards, checks that the text and the answers key and values are the same.
/// </summary>
public class ExactMatchEqualityChecker : ICardEqualityChecker
{
    /// <inheritdoc />
    public bool AreEqual(Card source, Card target)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (target == null) throw new ArgumentNullException(nameof(target));

        // Cards must be of the same type
        if (source.Type != target.Type)
        {
            return false;
        }

        return source.Type switch
        {
            CardType.QuestionAnswer => AreQuestionAnswerCardsEqual((QuestionAnswerCard)source, (QuestionAnswerCard)target),
            CardType.Cloze => AreClozeCardsEqual((ClozeCard)source, (ClozeCard)target),
            _ => throw new NotSupportedException($"Card type {source.Type} is not supported for equality checking.")
        };
    }

    private static bool AreQuestionAnswerCardsEqual(QuestionAnswerCard source, QuestionAnswerCard target)
    {
        // For QuestionAndAnswerCards, check if the question is exactly the same
        // Note: We don't check the answer because the design specifies only checking the question
        return string.Equals(source.Question, target.Question, StringComparison.Ordinal);
    }

    private static bool AreClozeCardsEqual(ClozeCard source, ClozeCard target)
    {
        // For ClozeCards, check that the text is exactly the same
        if (!string.Equals(source.Text, target.Text, StringComparison.Ordinal))
        {
            return false;
        }

        // Check that answers dictionaries have the same keys and values
        return DictionariesEqual(source.Answers, target.Answers);
    }

    private static bool DictionariesEqual<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        where TKey : notnull
    {
        if (dict1.Count != dict2.Count)
        {
            return false;
        }

        foreach (var kvp in dict1)
        {
            if (!dict2.TryGetValue(kvp.Key, out var value2) ||
                !EqualityComparer<TValue>.Default.Equals(kvp.Value, value2))
            {
                return false;
            }
        }

        return true;
    }
}