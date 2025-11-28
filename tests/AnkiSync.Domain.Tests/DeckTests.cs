using AnkiSync.Domain;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class DeckTests
{
    [Fact]
    public void Deck_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var deckId = DeckId.FromPath("TestDeck");
        var deck = new Deck { DeckId = deckId };

        // Assert
        deck.Id.Should().NotBeNull();
        deck.Id.Should().NotBe(Guid.Empty.ToString());
        deck.Name.Should().Be("TestDeck");
        deck.Cards.Should().BeEmpty();
    }

    [Fact]
    public void Deck_CanAddCards()
    {
        // Arrange
        var deckId = DeckId.FromPath("TestDeck");
        var deck = new Deck { DeckId = deckId };
        var qaCard = new QuestionAnswerCard { Id = Guid.NewGuid().ToString(), DateModified = DateTimeOffset.Now, Question = "Q", Answer = "A" };
        var clozeCard = new ClozeCard { Id = Guid.NewGuid().ToString(), DateModified = DateTimeOffset.Now, Text = "{{c1::test}}" };

        // Act
        deck.Cards.Add(qaCard);
        deck.Cards.Add(clozeCard);

        // Assert
        deck.Cards.Should().HaveCount(2);
        deck.Cards.Should().Contain(qaCard);
        deck.Cards.Should().Contain(clozeCard);
    }
}
