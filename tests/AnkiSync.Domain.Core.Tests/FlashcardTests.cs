using AnkiSync.Domain.Core;
using AnkiSync.Domain.Core.Exceptions;
using AnkiSync.Domain.Core.Extensions;

namespace AnkiSync.Domain.Core.Tests;

public class FlashcardTests
{
    [Fact]
    public void Flashcard_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var flashcard = new Flashcard();

        // Assert
        Assert.NotNull(flashcard.Id);
        Assert.NotEqual(Guid.Empty.ToString(), flashcard.Id);
        Assert.Equal(string.Empty, flashcard.Question);
        Assert.Equal(string.Empty, flashcard.Answer);
        Assert.Equal(FlashcardType.Basic, flashcard.Type);
        Assert.NotNull(flashcard.Tags);
        Assert.Empty(flashcard.Tags);
        Assert.Equal(string.Empty, flashcard.SourceFile);
        Assert.Equal(0, flashcard.LineNumber);
        Assert.True(flashcard.LastModified <= DateTime.UtcNow);
        Assert.Null(flashcard.AnkiNoteId);
        Assert.Equal(string.Empty, flashcard.InferredDeck);
    }

    [Fact]
    public void Flashcard_Validate_ThrowsWhenQuestionEmpty()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Question = string.Empty,
            Answer = "Answer",
            Tags = new List<string> { "#tag" },
            SourceFile = "test.md"
        };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => flashcard.Validate());
        Assert.Contains("question cannot be empty", exception.Message.ToLower());
    }

    [Fact]
    public void Flashcard_Validate_ThrowsWhenAnswerEmpty()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Question = "Question",
            Answer = string.Empty,
            Tags = new List<string> { "#tag" },
            SourceFile = "test.md"
        };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => flashcard.Validate());
        Assert.Contains("answer cannot be empty", exception.Message.ToLower());
    }

    [Fact]
    public void Flashcard_Validate_ThrowsWhenNoTags()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Question = "Question",
            Answer = "Answer",
            Tags = new List<string>(),
            SourceFile = "test.md"
        };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => flashcard.Validate());
        Assert.Contains("must have at least one tag", exception.Message.ToLower());
    }

    [Fact]
    public void Flashcard_Validate_ThrowsWhenSourceFileEmpty()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Question = "Question",
            Answer = "Answer",
            Tags = new List<string> { "#tag" },
            SourceFile = string.Empty
        };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => flashcard.Validate());
        Assert.Contains("source file cannot be empty", exception.Message.ToLower());
    }

    [Fact]
    public void Flashcard_GetFirstTag_ReturnsFirstTag()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Tags = new List<string> { "#first", "#second", "#third" }
        };

        // Act
        var firstTag = flashcard.GetFirstTag();

        // Assert
        Assert.Equal("#first", firstTag);
    }

    [Fact]
    public void Flashcard_GetFirstTag_ReturnsEmptyWhenNoTags()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            Tags = new List<string>()
        };

        // Act
        var firstTag = flashcard.GetFirstTag();

        // Assert
        Assert.Equal(string.Empty, firstTag);
    }

    [Fact]
    public void Flashcard_IsSynced_ReturnsFalseWhenNoAnkiNoteId()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            AnkiNoteId = null
        };

        // Act
        var isSynced = flashcard.IsSynced();

        // Assert
        Assert.False(isSynced);
    }

    [Fact]
    public void Flashcard_IsSynced_ReturnsTrueWhenHasAnkiNoteId()
    {
        // Arrange
        var flashcard = new Flashcard
        {
            AnkiNoteId = "12345"
        };

        // Act
        var isSynced = flashcard.IsSynced();

        // Assert
        Assert.True(isSynced);
    }
}