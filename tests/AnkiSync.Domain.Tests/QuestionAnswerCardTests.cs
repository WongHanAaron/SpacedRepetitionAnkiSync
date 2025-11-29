using AnkiSync.Domain;
using AnkiSync.Domain.Exceptions;
using AnkiSync.Domain.Extensions;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class QuestionAnswerCardTests
{
    [Fact]
    public void QuestionAnswerCard_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var card = new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.Now
        };

        // Assert
        card.DateModified.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
        card.Question.Should().BeEmpty();
        card.Answer.Should().BeEmpty();
        card.Type.Should().Be(CardType.QuestionAnswer);
    }

    [Fact]
    public void QuestionAnswerCard_Validate_ThrowsWhenQuestionEmpty()
    {
        // Arrange
        var card = new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.Now,
            Question = string.Empty,
            Answer = "Answer"
        };

        // Act & Assert
        card.Invoking(c => c.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("question cannot be empty");
    }

    [Fact]
    public void QuestionAnswerCard_Validate_ThrowsWhenAnswerEmpty()
    {
        // Arrange
        var card = new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.Now,
            Question = "Question",
            Answer = string.Empty
        };

        // Act & Assert
        card.Invoking(c => c.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("answer cannot be empty");
    }

    [Fact]
    public void QuestionAnswerCard_Validate_SucceedsWhenBothFieldsPresent()
    {
        // Arrange
        var card = new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.Now,
            Question = "What is the capital of France?",
            Answer = "Paris"
        };

        // Act & Assert
        card.Invoking(c => c.Validate())
            .Should().NotThrow();
    }
}
