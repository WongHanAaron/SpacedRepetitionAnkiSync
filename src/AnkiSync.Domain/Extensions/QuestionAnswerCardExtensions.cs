using AnkiSync.Domain.Exceptions;

namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Extension methods for QuestionAnswerCard validation and operations
/// </summary>
public static class QuestionAnswerCardExtensions
{
    /// <summary>
    /// Validates that a question-answer card has required fields
    /// </summary>
    /// <param name="card">The card to validate</param>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
    public static void Validate(this QuestionAnswerCard card)
    {
        if (string.IsNullOrWhiteSpace(card.Question))
        {
            throw new ValidationException("Question-answer card question cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(card.Answer))
        {
            throw new ValidationException("Question-answer card answer cannot be empty");
        }
    }
}