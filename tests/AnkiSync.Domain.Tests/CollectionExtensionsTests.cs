using AnkiSync.Domain;
using AnkiSync.Domain.Extensions;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class CollectionExtensionsTests
{
    [Fact]
    public void DistinctById_ReturnsDistinctCards()
    {
        // Arrange
        var card1 = new QuestionAnswerCard { Id = "1", Question = "Q1", Answer = "A1" };
        var card2 = new QuestionAnswerCard { Id = "2", Question = "Q2", Answer = "A2" };
        var card3 = new QuestionAnswerCard { Id = "1", Question = "Q3", Answer = "A3" }; // Duplicate ID
        var cards = new List<Card> { card1, card2, card3 };

        // Act
        var distinctCards = cards.DistinctById().ToList();

        // Assert
        distinctCards.Should().HaveCount(2);
        distinctCards.Should().Contain(card1);
        distinctCards.Should().Contain(card2);
        distinctCards.Should().NotContain(card3);
    }

    [Fact]
    public void DistinctById_HandlesEmptyCollection()
    {
        // Arrange
        var cards = new List<Card>();

        // Act
        var distinctCards = cards.DistinctById().ToList();

        // Assert
        distinctCards.Should().BeEmpty();
    }

    [Fact]
    public void DistinctById_HandlesMixedCardTypes()
    {
        // Arrange
        var qaCard = new QuestionAnswerCard { Id = "1", Question = "Q", Answer = "A" };
        var clozeCard = new ClozeCard { Id = "2", Text = "{{c1::test}}" };
        var duplicateQaCard = new QuestionAnswerCard { Id = "1", Question = "Q2", Answer = "A2" };
        var cards = new List<Card> { qaCard, clozeCard, duplicateQaCard };

        // Act
        var distinctCards = cards.DistinctById().ToList();

        // Assert
        distinctCards.Should().HaveCount(2);
        distinctCards.Should().Contain(qaCard);
        distinctCards.Should().Contain(clozeCard);
        distinctCards.Should().NotContain(duplicateQaCard);
    }
}