using AnkiSync.Domain;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class DeckIdentifierTests
{
    [Fact]
    public void DeckId_CanBeCreated_WithName()
    {
        // Arrange & Act
        var deckId = new DeckId { Name = "TestDeck" };

        // Assert
        deckId.Name.Should().Be("TestDeck");
    }
}