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
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Creates a new instance of FileParser
    /// </summary>
    /// <param name="fileSystem">The file system abstraction to use</param>
    public FileParser(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <summary>
    /// Parses a file into metadata and content
    /// </summary>
    /// <param name="filePath">The path to the file to parse</param>
    /// <returns>The document metadata</returns>
    public async Task<Document> ParseFileAsync(string filePath)
    {
        if (!_fileSystem.FileExists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }

        var fileInfo = _fileSystem.GetFileInfo(filePath);
        var content = await _fileSystem.ReadAllTextAsync(filePath);

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