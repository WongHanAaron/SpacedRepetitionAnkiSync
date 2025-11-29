using AnkiSync.Adapter.SpacedRepetitionNotes.Models;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Interface for parsing document content into documents
/// </summary>
public interface IFileParser
{
    /// <summary>
    /// Parses document content and extracts tags and metadata
    /// </summary>
    /// <param name="filePath">The path to the file (for metadata)</param>
    /// <param name="content">The content of the document to parse</param>
    /// <param name="lastModified">The last modified date of the file</param>
    /// <returns>A document containing the parsed content</returns>
    Task<Document> ParseContentAsync(string filePath, string content, DateTimeOffset lastModified);
}

/// <summary>
/// Implementation of IFileParser
/// </summary>
public class FileParser : IFileParser
{
    /// <summary>
    /// Creates a new instance of FileParser
    /// </summary>
    public FileParser()
    {
        // No longer needs file system dependency since content is passed in
    }

    /// <summary>
    /// Parses document content and extracts tags and metadata
    /// </summary>
    /// <param name="filePath">The path to the file (for metadata)</param>
    /// <param name="content">The content of the document to parse</param>
    /// <param name="lastModified">The last modified date of the file</param>
    /// <returns>A document containing the parsed content</returns>
    public Task<Document> ParseContentAsync(string filePath, string content, DateTimeOffset lastModified)
    {
        // Extract tags from content
        var tags = ExtractTags(content);

        var document = new Document
        {
            FilePath = filePath,
            LastModified = lastModified,
            Tags = new Tag { NestedTags = tags.ToList() },
            Content = content
        };

        return Task.FromResult(document);
    }

    private static IEnumerable<string> ExtractTags(string content)
    {
        // Extract only the first tag from content, separated by whitespace
        // Tags can contain '/' for nested hierarchy
        var firstMatch = System.Text.RegularExpressions.Regex.Match(content, @"#([^#\s]+)");
        if (firstMatch.Success)
        {
            var tagString = firstMatch.Groups[1].Value;
            return tagString.Split('/');
        }
        return Enumerable.Empty<string>();
    }
}
