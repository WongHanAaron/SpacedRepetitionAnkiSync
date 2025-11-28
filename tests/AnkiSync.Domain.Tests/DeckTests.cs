using AnkiSync.Domain;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class DeckTests
{
    [Fact]
    public void Deck_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var deck = new Deck();

        // Assert
        deck.Id.Should().NotBeNull();
        deck.Id.Should().NotBe(Guid.Empty.ToString());
        deck.Name.Should().BeEmpty();
        deck.Cards.Should().BeEmpty();
        deck.SubDeckNames.Should().BeEmpty();
    }

    [Fact]
    public void Deck_CanAddCards()
    {
        // Arrange
        var deck = new Deck();
        var qaCard = new QuestionAnswerCard { Question = "Q", Answer = "A" };
        var clozeCard = new ClozeCard { Text = "{{c1::test}}" };

        // Act
        deck.Cards.Add(qaCard);
        deck.Cards.Add(clozeCard);

        // Assert
        deck.Cards.Should().HaveCount(2);
        deck.Cards.Should().Contain(qaCard);
        deck.Cards.Should().Contain(clozeCard);
    }

    [Fact]
    public void Deck_CanAddSubDeckNames()
    {
        // Arrange
        var deck = new Deck();

        // Act
        deck.SubDeckNames.Add("SubDeck1");
        deck.SubDeckNames.Add("SubDeck2");

        // Assert
        deck.SubDeckNames.Should().HaveCount(2);
        deck.SubDeckNames.Should().Contain("SubDeck1");
        deck.SubDeckNames.Should().Contain("SubDeck2");
    }
}