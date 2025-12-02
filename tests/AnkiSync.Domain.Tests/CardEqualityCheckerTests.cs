using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using AnkiSync.Domain.Extensions;
using Xunit;

namespace AnkiSync.Domain.Tests;

public class CardEqualityCheckerTests
{
    [Fact]
    public void ICardEqualityChecker_InterfaceExists()
    {
        // This test will fail until we create the ICardEqualityChecker interface
        // Arrange & Act & Assert
        Assert.True(typeof(ICardEqualityChecker).IsInterface);
    }

    [Fact]
    public void ExactMatchEqualityChecker_CanBeCreated()
    {
        // This test will fail until we create the ExactMatchEqualityChecker class
        // Arrange & Act
        var checker = new ExactMatchEqualityChecker();

        // Assert
        Assert.NotNull(checker);
        Assert.IsAssignableFrom<ICardEqualityChecker>(checker);
    }

    [Fact]
    public void ExactMatchEqualityChecker_QuestionAnswerCards_MatchWhenQuestionSame()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card1 = new QuestionAnswerCard
        {
            Question = "What is 2+2?",
            Answer = "4",
            DateModified = DateTimeOffset.Now
        };
        var card2 = new QuestionAnswerCard
        {
            Question = "What is 2+2?",
            Answer = "Five", // Different answer
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(card1, card2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ExactMatchEqualityChecker_QuestionAnswerCards_DoNotMatchWhenQuestionDifferent()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card1 = new QuestionAnswerCard
        {
            Question = "What is 2+2?",
            Answer = "4",
            DateModified = DateTimeOffset.Now
        };
        var card2 = new QuestionAnswerCard
        {
            Question = "What is 3+3?",
            Answer = "6",
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(card1, card2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExactMatchEqualityChecker_ClozeCards_MatchWhenTextAndKeywordsSame()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card1 = new ClozeCard
        {
            Text = "The capital of France is {Paris}.",
            Answers = new Dictionary<string, string> { { "Paris", "capital city" } },
            DateModified = DateTimeOffset.Now
        };
        var card2 = new ClozeCard
        {
            Text = "The capital of France is {Paris}.",
            Answers = new Dictionary<string, string> { { "Paris", "capital city" } },
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(card1, card2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ExactMatchEqualityChecker_ClozeCards_DoNotMatchWhenTextDifferent()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card1 = new ClozeCard
        {
            Text = "The capital of France is {Paris}.",
            Answers = new Dictionary<string, string> { { "Paris", "capital city" } },
            DateModified = DateTimeOffset.Now
        };
        var card2 = new ClozeCard
        {
            Text = "Paris is the capital of France.",
            Answers = new Dictionary<string, string> { { "Paris", "capital city" } },
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(card1, card2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExactMatchEqualityChecker_ClozeCards_DoNotMatchWhenKeywordsDifferent()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card1 = new ClozeCard
        {
            Text = "The capital of France is {Paris}.",
            Answers = new Dictionary<string, string> { { "Paris", "capital city" } },
            DateModified = DateTimeOffset.Now
        };
        var card2 = new ClozeCard
        {
            Text = "The capital of France is {Paris}.",
            Answers = new Dictionary<string, string> { { "Paris", "city" } }, // Different answer value
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(card1, card2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExactMatchEqualityChecker_ThrowsForNullCards()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card = new QuestionAnswerCard
        {
            Question = "Test",
            Answer = "Answer",
            DateModified = DateTimeOffset.Now
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => checker.AreEqual(null!, card));
        Assert.Throws<ArgumentNullException>(() => checker.AreEqual(card, null!));
    }

    [Fact]
    public void ExactMatchEqualityChecker_DifferentCardTypes_DoNotMatch()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var qaCard = new QuestionAnswerCard
        {
            Question = "What is 2+2?",
            Answer = "4",
            DateModified = DateTimeOffset.Now
        };
        var clozeCard = new ClozeCard
        {
            Text = "2+2 equals {four}.",
            Answers = new Dictionary<string, string> { { "four", "4" } },
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(qaCard, clozeCard);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExactMatchEqualityChecker_ClozeCards_OrderOfAnswersDoesNotMatter()
    {
        // Arrange
        var checker = new ExactMatchEqualityChecker();
        var card1 = new ClozeCard
        {
            Text = "The {color} {animal} jumped over the {object}.",
            Answers = new Dictionary<string, string>
            {
                { "color", "brown" },
                { "animal", "fox" },
                { "object", "fence" }
            },
            DateModified = DateTimeOffset.Now
        };
        var card2 = new ClozeCard
        {
            Text = "The {color} {animal} jumped over the {object}.",
            Answers = new Dictionary<string, string>
            {
                { "animal", "fox" }, // Different order
                { "object", "fence" },
                { "color", "brown" }
            },
            DateModified = DateTimeOffset.Now
        };

        // Act
        var result = checker.AreEqual(card1, card2);

        // Assert
        Assert.True(result);
    }
}