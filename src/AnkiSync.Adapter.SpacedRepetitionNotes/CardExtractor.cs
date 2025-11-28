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
    IEnumerable<ParsedCard> ExtractCards(Document document);
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
    public IEnumerable<ParsedCard> ExtractCards(Document document)
    {
        var cards = new List<ParsedCard>();

        // Extract basic cards (Q: ... A: ...)
        cards.AddRange(ExtractBasicCards(document));

        // Extract cloze cards ({{c1::text}})
        cards.AddRange(ExtractClozeCards(document));

        return cards;
    }

    private IEnumerable<ParsedCard> ExtractBasicCards(Document document)
    {
        var lines = document.Content.Split('\n');
        var cards = new List<ParsedCard>();
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
                cards.Add(new ParsedCard
                {
                    Front = currentQuestion,
                    Back = answer,
                    Tags = document.Tags,
                    SourceFilePath = document.FilePath,
                    CardType = "Basic"
                });
                currentQuestion = null;
            }
        }

        return cards;
    }

    private IEnumerable<ParsedCard> ExtractClozeCards(Document document)
    {
        // Extract cloze patterns like {{c1::text}} or {{keyword::text}}
        var clozePattern = @"{{(?:c(\d+)|([^:]+))::([^}]+)}}";
        var matches = System.Text.RegularExpressions.Regex.Matches(document.Content, clozePattern);

        if (matches.Count > 0)
        {
            // Extract answers and create placeholder text
            var answers = new Dictionary<string, string>();
            var placeholderText = document.Content;
            var placeholderIndex = 0;

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
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
                    keyword = $"answer{++placeholderIndex}";
                }

                answers[keyword] = answer;

                // Replace the cloze with a named placeholder
                var placeholder = $"{{{keyword}}}";
                placeholderText = placeholderText.Replace(match.Value, placeholder);
            }

            yield return new ParsedCard
            {
                Front = placeholderText,
                Back = placeholderText, // Cloze cards show the full text
                Tags = document.Tags,
                SourceFilePath = document.FilePath,
                CardType = "Cloze",
                Answers = answers
            };
        }
    }
}