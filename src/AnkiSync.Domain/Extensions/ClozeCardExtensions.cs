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

        // Extract named placeholders from the text (e.g., {country}, {city})
        var placeholderPattern = @"\{([^}]+)\}";
        var matches = System.Text.RegularExpressions.Regex.Matches(card.Text, placeholderPattern);
        var placeholdersInText = new HashSet<string>();

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (match.Groups.Count >= 2)
            {
                placeholdersInText.Add(match.Groups[1].Value);
            }
        }

        if (placeholdersInText.Count == 0)
        {
            throw new ValidationException("Cloze card must contain named placeholders in the format {name}");
        }

        // Check that all placeholders have corresponding answers
        foreach (var placeholder in placeholdersInText)
        {
            if (!card.Answers.ContainsKey(placeholder))
            {
                throw new ValidationException($"Cloze card is missing answer for placeholder '{placeholder}'");
            }
        }

        // Check that all answers are non-empty
        foreach (var answer in card.Answers)
        {
            if (string.IsNullOrWhiteSpace(answer.Value))
            {
                throw new ValidationException($"Cloze card answer for placeholder '{answer.Key}' cannot be empty");
            }
        }

        // Check for extra answers that don't have placeholders
        foreach (var answerKey in card.Answers.Keys)
        {
            if (!placeholdersInText.Contains(answerKey))
            {
                throw new ValidationException($"Cloze card has answer for '{answerKey}' but no corresponding placeholder in text");
            }
        }
    }
}