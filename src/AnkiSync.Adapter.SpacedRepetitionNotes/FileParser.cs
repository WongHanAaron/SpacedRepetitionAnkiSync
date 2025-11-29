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
        // Split content into lines
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Only check the first 2 lines
        for (int i = 0; i < Math.Min(2, lines.Length); i++)
        {
            var line = lines[i].Trim();
            
            // Check if line starts with # (after trimming whitespace)
            if (line.StartsWith("#"))
            {
                // Use regex to find the first valid tag on this line
                // Same logic as original: (?<!#)#([^#\s]+)
                var match = System.Text.RegularExpressions.Regex.Match(line, @"(?<!#)#([^#\s]+)");
                if (match.Success)
                {
                    var tagString = match.Groups[1].Value;
                    
                    // Split by '/' for nested tags and return all parts
                    var tagParts = tagString.Split('/', StringSplitOptions.RemoveEmptyEntries)
                        .Select(part => part.Trim())
                        .Where(part => !string.IsNullOrEmpty(part))
                        .ToArray();
                    
                    if (tagParts.Length > 0)
                    {
                        return tagParts;
                    }
                }
            }
        }
        
        return Enumerable.Empty<string>();
    }
}
