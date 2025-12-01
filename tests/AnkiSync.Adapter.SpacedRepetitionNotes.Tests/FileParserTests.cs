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
    public async Task ParseContentAsync_WithNestedTags_ShouldExtractAllTagParts()
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
    public async Task ParseContentAsync_WithMixedTagFormats_ShouldExtractFromFirstLine()
    {
        // Arrange
        var content = @"#cloud #aws/compute #simple-tag
#another/tag #final
Some content here.";
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
    public async Task ParseContentAsync_WithSpaceAfterHash_ShouldNotExtractTag()
    {
        // Arrange
        var content = @"# test #valid
Content with space after hash that should not be extracted.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "valid" });
    }

    [Fact]
    public async Task ParseContentAsync_WithMultipleHashes_ShouldNotExtractTag()
    {
        // Arrange
        var content = @"##invalid ###alsoinvalid #valid  Content with multiple hashes that should not be extracted.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "valid" });
    }

    [Fact]
    public async Task ParseContentAsync_WithDeeplyNestedTag_ShouldExtractAllParts()
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

    [Fact]
    public async Task ParseContentAsync_WithTagContainingHyphens_ShouldTreatAsSingleTag()
    {
        // Arrange
        var content = @"#test-okthen-then  Content with tag containing hyphens.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "test-okthen-then" });
    }

    [Fact]
    public async Task ParseContentAsync_WithTagContainingInvalidCharacters_ShouldSkipInvalidTags()
    {
        // Arrange
        var content = @"#tag@invalid #valid-tag
Content with invalid characters in tag.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert - #tag@invalid is ignored entirely, so #valid-tag is the first valid tag
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "valid-tag" });
    }

    [Fact]
    public async Task ParseContentAsync_WithTagContainingUnderscores_ShouldAcceptUnderscores()
    {
        // Arrange
        var content = @"#valid_tag #another_valid
Content with underscores in tags.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "valid_tag" });
    }

    [Fact]
    public async Task ParseContentAsync_WithTagContainingNumbers_ShouldAcceptNumbers()
    {
        // Arrange
        var content = @"#tag123 #test456
Content with numbers in tags.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "tag123" });
    }

    [Fact]
    public async Task ParseContentAsync_WithTagContainingSpecialCharacters_ShouldSkipInvalidTags()
    {
        // Arrange
        var content = @"#tag$invalid #tag#also#invalid #valid/tag
Content with special characters in tags.";
        var filePath = "test.md";
        var lastModified = DateTimeOffset.UtcNow;

        // Act
        var document = await _fileParser.ParseContentAsync(filePath, content, lastModified);

        // Assert - #tag$invalid and #tag#also#invalid are ignored, so #valid/tag is the first valid tag
        document.Tags.NestedTags.Should().BeEquivalentTo(new[] { "valid", "tag" });
    }
}
