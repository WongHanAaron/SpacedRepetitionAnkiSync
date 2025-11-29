using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using AnkiSync.Domain;

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
        // Split by both Unix and Windows line endings, and keep empty entries
        var lines = document.Content
            .Replace("\r\n", "\n")  // Normalize Windows line endings
            .Split('\n', StringSplitOptions.None);

        // First, try to extract single-line cards from all non-empty, non-comment lines
        var nonEmptyLines = lines
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("#"))
            .ToArray();

        var cards = new List<ParsedCardBase>();
        var singleCards = ExtractSingleLineFlashcards(nonEmptyLines, document).ToList();
        cards.AddRange(singleCards);
        if (!singleCards.Any())
        {
            var reversedCards = ExtractReversedFlashcards(nonEmptyLines, document).ToList();
            cards.AddRange(reversedCards);
            cards.AddRange(ExtractMultiLineFlashcards(lines, document));
        }
        return cards;
    }

    private IEnumerable<ParsedCardBase> ExtractSingleLineFlashcards(string[] lines, Document document)
    {
        Console.WriteLine($"Extracting cards from {lines.Length} lines");
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();
            Console.WriteLine($"Processing line {i}: '{trimmed}'");
            
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
            {
                Console.WriteLine("Skipping empty line or comment");
                continue;
            }

            if (trimmed.Contains("{{") || trimmed.Contains("==") || trimmed.Contains("**"))
            {
                Console.WriteLine("Skipping line with cloze markers");
                continue;
            }

            if (trimmed.Contains(":::"))
            {
                Console.WriteLine("Skipping line with reversed card marker");
                continue;
            }

            // First try to split by "?::" to handle the case where the question ends with a question mark
            var parts = trimmed.Split(new[] { "?::" }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var question = parts[0].Trim();
                var answer = parts[1].Trim();
                Console.WriteLine($"Found potential card with '?::' separator - Q: '{question}', A: '{answer}'");

                if (!string.IsNullOrWhiteSpace(question))
                {
                    // Add back the question mark if it's not already there
                    if (!question.EndsWith("?"))
                    {
                        question += "?";
                        Console.WriteLine("Added missing question mark");
                    }

                    var q = string.IsNullOrWhiteSpace(answer) ? trimmed : question;
                    var card = new ParsedQuestionAnswerCard
                    {
                        Question = q,
                        Answer = answer,
                        Tags = document.Tags,
                        SourceFilePath = document.FilePath
                    };
                    Console.WriteLine($"Yielding card: Q: {card.Question}, A: {card.Answer}");
                    yield return card;
                    continue; // Move to next line after processing this card
                }
            }
            else
            {
                Console.WriteLine("No '?::' separator found, trying '::'");
            }

            // If no match with "?::", try splitting by just "::"
            parts = trimmed.Split(new[] { "::" }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var question = parts[0].Trim();
                var answer = parts[1].Trim();
                Console.WriteLine($"Found potential card with '::' separator - Q: '{question}', A: '{answer}'");

                // Only consider it a valid card if the question ends with a question mark
                if (!string.IsNullOrWhiteSpace(question))
                {
                    var q = string.IsNullOrWhiteSpace(answer) ? trimmed : question;
                    var card = new ParsedQuestionAnswerCard
                    {
                        Question = q,
                        Answer = answer,
                        Tags = document.Tags,
                        SourceFilePath = document.FilePath
                    };
                    Console.WriteLine($"Yielding card: Q: {card.Question}, A: {card.Answer}");
                    yield return card;
                }
                else
                {
                    Console.WriteLine("Skipping card - question is invalid");
                }
            }
            else
            {
                Console.WriteLine("No valid card format found on this line");
            }
        }
    }

    private IEnumerable<ParsedCardBase> ExtractReversedFlashcards(string[] lines, Document document)
    {
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                continue;

            // Split the line by triple colons to find reversed question-answer pairs
            var segments = trimmed.Split(new[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 2)
            {
                var question = segments[0].Trim();
                var answer = segments[1].Trim();

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

                    // Reversed card: Answer -> Question
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
            else if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("#") && !trimmed.Contains("{{") && !trimmed.Contains("==") && !trimmed.Contains("**"))
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

        if (question.Contains("{{") || question.Contains("==") || question.Contains("**"))
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
        var lines = document.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            // Skip lines that are just tags or comments
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
            {
                continue;
            }

            // Check if this line has cloze markers
            if (trimmed.Contains("{{") || trimmed.Contains("==") || trimmed.Contains("**"))
            {
                var answers = new Dictionary<string, string>();
                var placeholderText = trimmed;
                var clozeFound = false;
                var unnamedClozeIndex = 1;

                // Handle Anki-style cloze: {{c1::text}} or {{keyword::text}}
                var ankiClozePattern = @"{{(?:c(\d+)|([^:]+))::([^}]+)}}";
                var ankiMatches = System.Text.RegularExpressions.Regex.Matches(trimmed, ankiClozePattern);
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
                        keyword = $"answer{unnamedClozeIndex++}";
                    }

                    answers[keyword] = answer;

                    // Replace the cloze with a named placeholder
                    var placeholder = $"{{{keyword}}}";
                    placeholderText = placeholderText.Replace(match.Value, placeholder);
                }

                // Handle Obsidian-style cloze deletions
                // ==highlight== format
                var highlightPattern = @"==(.*?)==(?!\s*=)"; // Avoid matching === or more
                var highlightMatches = System.Text.RegularExpressions.Regex.Matches(trimmed, highlightPattern);

                foreach (System.Text.RegularExpressions.Match match in highlightMatches)
                {
                    clozeFound = true;
                    var answer = match.Groups[1].Value;
                    var keyword = $"answer{unnamedClozeIndex++}";
                    answers[keyword] = answer;

                    // Replace with named placeholder
                    var placeholder = $"{{{keyword}}}";
                    placeholderText = placeholderText.Replace(match.Value, placeholder);
                }

                // **bolded text** format
                var boldPattern = @"\*\*(.*?)\*\*(?!\s*\*)"; // Avoid matching *** or more
                var boldMatches = System.Text.RegularExpressions.Regex.Matches(trimmed, boldPattern);

                foreach (System.Text.RegularExpressions.Match match in boldMatches)
                {
                    clozeFound = true;
                    var answer = match.Groups[1].Value;
                    var keyword = $"answer{unnamedClozeIndex++}";
                    answers[keyword] = answer;

                    // Replace the cloze with a named placeholder
                    var placeholder = $"{{{keyword}}}";
                    placeholderText = placeholderText.Replace(match.Value, placeholder);
                }

                // {{text in curly braces}} format (different from Anki {{c1::text}})
                var curlyPattern = @"{{(?!.*::)([^}]+?)}}"; // Match {{text}} but not {{c1::text}}
                var curlyMatches = System.Text.RegularExpressions.Regex.Matches(trimmed, curlyPattern);

                foreach (System.Text.RegularExpressions.Match match in curlyMatches)
                {
                    clozeFound = true;
                    var answer = match.Groups[1].Value;
                    var keyword = $"answer{unnamedClozeIndex++}";
                    answers[keyword] = answer;

                    // Replace the cloze with a named placeholder
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
    }
}