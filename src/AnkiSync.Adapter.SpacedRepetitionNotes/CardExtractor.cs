using AnkiSync.Adapter.SpacedRepetitionNotes.Models;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Interface for extracting cards from document content
/// </summary>
public interface ICardExtractor
{
    /// <summary>
    /// Extracts cards from document metadata
    /// </summary>
    /// <param name="document">The document metadata containing content</param>
    /// <returns>The extracted cards</returns>
    IEnumerable<ParsedCardBase> ExtractCards(Document document);
}

/// <summary>
/// Implementation of ICardExtractor
/// </summary>
public class CardExtractor : ICardExtractor
{
    /// <summary>
    /// Extracts cards from document metadata
    /// </summary>
    /// <param name="document">The document metadata containing content</param>
    /// <returns>The extracted cards</returns>
    public IEnumerable<ParsedCardBase> ExtractCards(Document document)
    {
        var cards = new List<ParsedCardBase>();

        // Extract Obsidian-style flashcards
        cards.AddRange(ExtractObsidianFlashcards(document));

        // Extract legacy basic cards (Q: ... A: ...)
        cards.AddRange(ExtractBasicCards(document));

        // Extract cloze cards ({{c1::text}}, ==highlight==, **bold**, {{curly}})
        cards.AddRange(ExtractClozeCards(document));

        return cards;
    }

    private IEnumerable<ParsedCardBase> ExtractObsidianFlashcards(Document document)
    {
        var cards = new List<ParsedCardBase>();
        var lines = document.Content.Split('\n');

        // Extract single-line flashcards (Question::Answer and Question:::Answer)
        cards.AddRange(ExtractSingleLineFlashcards(lines, document));

        // Extract multi-line flashcards (separated by ? and ??)
        cards.AddRange(ExtractMultiLineFlashcards(lines, document));

        return cards;
    }

    private IEnumerable<ParsedCardBase> ExtractSingleLineFlashcards(string[] lines, Document document)
    {
        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Single-line basic: Question::Answer
            if (trimmed.Contains("::") && !trimmed.Contains(":::"))
            {
                var parts = trimmed.Split(new[] { "::" }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    var question = parts[0].Trim();
                    var answer = parts[1].Trim();

                    if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
                    {
                        yield return new ParsedQuestionAnswerCard
                        {
                            Question = question,
                            Answer = answer,
                            Tags = document.Tags,
                            SourceFilePath = document.FilePath
                        };
                    }
                }
            }
            // Single-line reversed: Question:::Answer (creates two cards)
            else if (trimmed.Contains(":::"))
            {
                var parts = trimmed.Split(new[] { ":::" }, 2, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    var question = parts[0].Trim();
                    var answer = parts[1].Trim();

                    if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
                    {
                        // Forward card: Question -> Answer
                        yield return new ParsedQuestionAnswerCard
                        {
                            Question = question,
                            Answer = answer,
                            Tags = document.Tags,
                            SourceFilePath = document.FilePath
                        };

                        // Reverse card: Answer -> Question
                        yield return new ParsedQuestionAnswerCard
                        {
                            Question = answer,
                            Answer = question,
                            Tags = document.Tags,
                            SourceFilePath = document.FilePath
                        };
                    }
                }
            }
        }
    }

    private IEnumerable<ParsedCardBase> ExtractMultiLineFlashcards(string[] lines, Document document)
    {
        var currentCard = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Check for multi-line separators
            if (trimmed == "?")
            {
                // Basic multi-line card
                if (currentCard.Count > 0)
                {
                    foreach (var card in CreateMultiLineCard(currentCard, false, document))
                    {
                        yield return card;
                    }
                    currentCard.Clear();
                }
            }
            else if (trimmed == "??")
            {
                // Reversed multi-line card
                if (currentCard.Count > 0)
                {
                    foreach (var card in CreateMultiLineCard(currentCard, true, document))
                    {
                        yield return card;
                    }
                    currentCard.Clear();
                }
            }
            else if (!string.IsNullOrWhiteSpace(trimmed))
            {
                currentCard.Add(line); // Keep original formatting
            }
        }

        // Handle card at end of file
        if (currentCard.Count > 0)
        {
            foreach (var card in CreateMultiLineCard(currentCard, false, document))
            {
                yield return card;
            }
        }
    }

    private IEnumerable<ParsedCardBase> CreateMultiLineCard(List<string> lines, bool isReversed, Document document)
    {
        if (lines.Count < 2) yield break;

        // First line is question, rest are answer
        var question = lines[0].Trim();
        var answer = string.Join("\n", lines.Skip(1)).Trim();

        if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(answer))
            yield break;

        // Forward card
        yield return new ParsedQuestionAnswerCard
        {
            Question = question,
            Answer = answer,
            Tags = document.Tags,
            SourceFilePath = document.FilePath
        };

        // Reverse card if requested
        if (isReversed)
        {
            yield return new ParsedQuestionAnswerCard
            {
                Question = answer,
                Answer = question,
                Tags = document.Tags,
                SourceFilePath = document.FilePath
            };
        }
    }

    private IEnumerable<ParsedCardBase> ExtractBasicCards(Document document)
    {
        var lines = document.Content.Split('\n');
        var cards = new List<ParsedCardBase>();
        string? currentQuestion = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("Q:") || trimmed.StartsWith("Question:"))
            {
                currentQuestion = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
            }
            else if ((trimmed.StartsWith("A:") || trimmed.StartsWith("Answer:")) && currentQuestion != null)
            {
                var answer = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
                cards.Add(new ParsedQuestionAnswerCard
                {
                    Question = currentQuestion,
                    Answer = answer,
                    Tags = document.Tags,
                    SourceFilePath = document.FilePath
                });
                currentQuestion = null;
            }
        }

        return cards;
    }

    private IEnumerable<ParsedCardBase> ExtractClozeCards(Document document)
    {
        var answers = new Dictionary<string, string>();
        var placeholderText = document.Content;
        var clozeFound = false;

        // Handle Anki-style cloze: {{c1::text}} or {{keyword::text}}
        var ankiClozePattern = @"{{(?:c(\d+)|([^:]+))::([^}]+)}}";
        var ankiMatches = System.Text.RegularExpressions.Regex.Matches(document.Content, ankiClozePattern);

        foreach (System.Text.RegularExpressions.Match match in ankiMatches)
        {
            clozeFound = true;
            var answer = match.Groups[3].Value;
            string keyword;

            if (match.Groups[1].Success)
            {
                // This is a numbered cloze like {{c1::text}}, convert to named keyword
                keyword = $"answer{match.Groups[1].Value}";
            }
            else if (match.Groups[2].Success)
            {
                // This is a named keyword like {{country::France}}
                keyword = match.Groups[2].Value;
            }
            else
            {
                // Fallback for unexpected format
                keyword = $"answer{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            answers[keyword] = answer;

            // Replace the cloze with a named placeholder
            var placeholder = $"{{{keyword}}}";
            placeholderText = placeholderText.Replace(match.Value, placeholder);
        }

        // Handle Obsidian-style cloze deletions
        // ==highlight== format
        var highlightPattern = @"==(.*?)==(?!\s*=)"; // Avoid matching === or more
        var highlightMatches = System.Text.RegularExpressions.Regex.Matches(document.Content, highlightPattern);

        foreach (System.Text.RegularExpressions.Match match in highlightMatches)
        {
            clozeFound = true;
            var answer = match.Groups[1].Value;
            var keyword = $"cloze{Guid.NewGuid().ToString().Substring(0, 8)}";
            answers[keyword] = answer;

            // Replace with named placeholder
            var placeholder = $"{{{keyword}}}";
            placeholderText = placeholderText.Replace(match.Value, placeholder);
        }

        // **bolded text** format
        var boldPattern = @"\*\*(.*?)\*\*(?!\s*\*)"; // Avoid matching *** or more
        var boldMatches = System.Text.RegularExpressions.Regex.Matches(document.Content, boldPattern);

        foreach (System.Text.RegularExpressions.Match match in boldMatches)
        {
            clozeFound = true;
            var answer = match.Groups[1].Value;
            var keyword = $"cloze{Guid.NewGuid().ToString().Substring(0, 8)}";
            answers[keyword] = answer;

            // Replace with named placeholder
            var placeholder = $"{{{keyword}}}";
            placeholderText = placeholderText.Replace(match.Value, placeholder);
        }

        // {{text in curly braces}} format (different from Anki {{c1::text}})
        var curlyPattern = @"{{([^}]+?)}}(?!:)"; // Match {{text}} but not {{c1::text}}
        var curlyMatches = System.Text.RegularExpressions.Regex.Matches(document.Content, curlyPattern);

        foreach (System.Text.RegularExpressions.Match match in curlyMatches)
        {
            clozeFound = true;
            var answer = match.Groups[1].Value;
            var keyword = $"cloze{Guid.NewGuid().ToString().Substring(0, 8)}";
            answers[keyword] = answer;

            // Replace with named placeholder
            var placeholder = $"{{{keyword}}}";
            placeholderText = placeholderText.Replace(match.Value, placeholder);
        }

        if (clozeFound && answers.Count > 0)
        {
            yield return new ParsedClozeCard
            {
                Text = placeholderText,
                Answers = answers,
                Tags = document.Tags,
                SourceFilePath = document.FilePath
            };
        }
    }
}