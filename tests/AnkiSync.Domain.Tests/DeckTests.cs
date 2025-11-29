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
        deck.Name.Should().Be("TestDeck");
        deck.Cards.Should().BeEmpty();
    }

    [Fact]
    public void Deck_CanAddCards()
    {
        // Arrange
        var deckId = DeckId.FromPath("TestDeck");
        var deck = new Deck { DeckId = deckId };
        var qaCard = new QuestionAnswerCard { DateModified = DateTimeOffset.Now, Question = "Q", Answer = "A" };
        var clozeCard = new ClozeCard { DateModified = DateTimeOffset.Now, Text = """Test {keyword}""", Answers = new Dictionary<string, string> { ["keyword"] = "value" } };

        // Act
        deck.Cards.Add(qaCard);
        deck.Cards.Add(clozeCard);

        // Assert
        deck.Cards.Should().HaveCount(2);
        deck.Cards.Should().Contain(qaCard);
        deck.Cards.Should().Contain(clozeCard);
    }
}
