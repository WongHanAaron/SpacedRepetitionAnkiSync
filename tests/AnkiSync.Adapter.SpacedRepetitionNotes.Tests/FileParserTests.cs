using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using FluentAssertions;
using System.IO;
using Xunit;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Tests;

/// <summary>
/// Simple implementation of IFileSystem for testing
/// </summary>
public class FileSystemAdapter : IFileSystem
{
    public bool FileExists(string path) => File.Exists(path);
    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);
    public IFileInfo GetFileInfo(string path) => new FileInfoAdapter(new FileInfo(path));
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) => Directory.GetFiles(path, searchPattern, searchOption);
    public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
}

/// <summary>
/// Simple implementation of IFileInfo for testing
/// </summary>
public class FileInfoAdapter : IFileInfo
{
    private readonly FileInfo _fileInfo;

    public FileInfoAdapter(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
    }

    public IDirectoryInfo? Directory => _fileInfo.Directory != null ? new DirectoryInfoAdapter(_fileInfo.Directory) : null;
    public DateTimeOffset LastWriteTimeUtc => _fileInfo.LastWriteTimeUtc;
}

/// <summary>
/// Simple implementation of IDirectoryInfo for testing
/// </summary>
public class DirectoryInfoAdapter : IDirectoryInfo
{
    private readonly DirectoryInfo _directoryInfo;

    public DirectoryInfoAdapter(DirectoryInfo directoryInfo)
    {
        _directoryInfo = directoryInfo;
    }

    public string Name => _directoryInfo.Name;
    public IDirectoryInfo? Parent => _directoryInfo.Parent != null ? new DirectoryInfoAdapter(_directoryInfo.Parent) : null;
}

public class FileParserTests
{
    private readonly FileParser _fileParser;

    public FileParserTests()
    {
        _fileParser = new FileParser();
    }

    [Fact]
    public async Task ParseContentAsync_WithMultipleTagsOnSameLine_ShouldExtractFirstTag()
    {
        // Arrange
        var content = @"#cloud #aws #compute

This is content with multiple tags on the same line.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "cloud" });
        document.FilePath.Should().Be(filePath);
        document.LastModified.Should().Be(lastModified);
        document.Content.Should().Be(content);
    }

    [Fact]
    public async Task ParseContentAsync_WithNestedTags_ShouldExtractFirstTagSplitBySlashes()
    {
        // Arrange
        var content = @"#algorithms/datastructures #aws/compute/ec2

Content with nested tags.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "algorithms", "datastructures" });
    }

    [Fact]
    public async Task ParseContentAsync_WithTagsOnMultipleLines_ShouldExtractFirstTag()
    {
        // Arrange
        var content = @"#cloud
#aws
#compute

Content with tags on separate lines.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "cloud" });
    }

    [Fact]
    public async Task ParseContentAsync_WithMixedTagFormats_ShouldExtractFirstValidTag()
    {
        // Arrange
        var content = @"#cloud #aws/compute #simple-tag

Some content here.

#another/tag #final

More content.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "cloud" });
    }

    [Fact]
    public async Task ParseContentAsync_WithDuplicateTags_ShouldReturnFirstTag()
    {
        // Arrange  
        var content = @"#cloud #aws #cloud

Content with duplicate tags.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "cloud" });
    }

    [Fact]
    public async Task ParseContentAsync_WithNoTags_ShouldReturnEmptyTagList()
    {
        // Arrange
        var content = @"This is content without any tags.

Just plain text.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseContentAsync_WithInvalidTagFormats_ShouldExtractFirstValidTag()
    {
        // Arrange
        var content = @"#valid #another-valid

Content with invalid tag formats like invalid tag and invalidhtag that should be skipped.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "valid" });
    }

    [Fact]
    public async Task ParseContentAsync_WithDeeplyNestedTag_ShouldSplitBySlashes()
    {
        // Arrange
        var content = @"#aws/compute/ec2

Content with deeply nested tag.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "aws", "compute", "ec2" });
    }
}