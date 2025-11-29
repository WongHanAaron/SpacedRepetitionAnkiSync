using AnkiSync.Adapter.SpacedRepetitionNotes;
using FluentAssertions;
using System.IO.Abstractions;
using Xunit;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Tests;

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
        var content = @"#cloud #aws #compute  This is content with multiple tags on the same line.";
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
        var content = @"#algorithms/datastructures #aws/compute/ec2  Content with nested tags.";
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
        var content = @"#cloud #aws/compute #simple-tag  Some content here.
#another/tag #final  More content.";
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
        var content = @"#cloud #aws #cloud  Content with duplicate tags.";
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
        var content = @"This is content without any tags.  Just plain text.";
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
        var content = @"#valid #another-valid  Content with invalid tag formats like invalid tag and invalidhtag that should be skipped.";
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
        var content = @"#aws/compute/ec2  Content with deeply nested tag.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "aws", "compute", "ec2" });
    }
}
