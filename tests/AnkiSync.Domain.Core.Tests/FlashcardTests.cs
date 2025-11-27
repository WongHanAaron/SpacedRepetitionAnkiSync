using AnkiSync.Domain.Core;
using AnkiSync.Domain.Core.Exceptions;
using AnkiSync.Domain.Core.Extensions;
using FluentAssertions;

namespace AnkiSync.Domain.Core.Tests;

public class FlashcardTests
{
    [Fact]
    public void Flashcard_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var flashcard = new Flashcard();

        // Assert
        flashcard.Id.Should().NotBeNull();
        flashcard.Id.Should().NotBe(Guid.Empty.ToString());
        flashcard.Question.Should().BeEmpty();
        flashcard.Answer.Should().BeEmpty();
        flashcard.Type.Should().Be(FlashcardType.Basic);
        flashcard.Tags.Should().NotBeNull();
        flashcard.Tags.Should().BeEmpty();
        flashcard.SourceFile.Should().BeEmpty();
        flashcard.LineNumber.Should().Be(0);
        flashcard.LastModified.Should().BeOnOrBefore(DateTime.UtcNow);
        flashcard.AnkiNoteId.Should().BeNull();
        flashcard.InferredDeck.Should().BeEmpty();
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
        flashcard.Invoking(f => f.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("question cannot be empty");
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
        flashcard.Invoking(f => f.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("answer cannot be empty");
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
        flashcard.Invoking(f => f.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("must have at least one tag");
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
        flashcard.Invoking(f => f.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("source file cannot be empty");
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
        firstTag.Should().Be("#first");
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
        firstTag.Should().Be(string.Empty);
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
        isSynced.Should().BeFalse();
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
        isSynced.Should().BeTrue();
    }
}