using AnkiSync.Adapter.SpacedRepetitionNotes.Models;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Interface for parsing files into metadata
/// </summary>
public interface IFileParser
{
    /// <summary>
    /// Parses a file into metadata and content
    /// </summary>
    /// <param name="filePath">The path to the file to parse</param>
    /// <returns>The document metadata</returns>
    Task<Document> ParseFileAsync(string filePath);
}

/// <summary>
/// Implementation of IFileParser
/// </summary>
public class FileParser : IFileParser
{
    /// <summary>
    /// Parses a file into metadata and content
    /// </summary>
    /// <param name="filePath">The path to the file to parse</param>
    /// <returns>The document metadata</returns>
    public async Task<Document> ParseFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }

        var fileInfo = new FileInfo(filePath);
        var content = await File.ReadAllTextAsync(filePath);

        // Extract tags from content (simple implementation - look for #tags)
        var tags = ExtractTags(content);

        return new Document
        {
            FilePath = filePath,
            LastModified = fileInfo.LastWriteTimeUtc,
            Tags = new Tag { NestedTags = tags.ToList() },
            Content = content
        };
    }

    private static IEnumerable<string> ExtractTags(string content)
    {
        // Simple tag extraction - find words starting with #
        var tagMatches = System.Text.RegularExpressions.Regex.Matches(content, @"#(\w+)");
        return tagMatches.Select(m => m.Groups[1].Value).Distinct();
    }
}