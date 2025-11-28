using AnkiSync.Application;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnkiSync.Domain.Tests;

public class CardSynchronizationServiceTests
{
    private readonly Mock<ICardSourceRepository> _cardSourceRepositoryMock;
    private readonly Mock<IDeckRepository> _deckRepositoryMock;
    private readonly Mock<ILogger<CardSynchronizationService>> _loggerMock;
    private readonly CardSynchronizationService _synchronizationService;

    public CardSynchronizationServiceTests()
    {
        _cardSourceRepositoryMock = new Mock<ICardSourceRepository>();
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _loggerMock = new Mock<ILogger<CardSynchronizationService>>();
        _synchronizationService = new CardSynchronizationService(
            _cardSourceRepositoryMock.Object,
            _deckRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SynchronizeCardsAsync_WithNewDeck_CreatesDeck()
    {
        // Arrange
        var directories = new[] { "test/dir" };
        var sourceDeck = CreateTestDeck("TestDeck", "Question?", "Answer!");
        
        _cardSourceRepositoryMock
            .Setup(x => x.GetCardsFromDirectories(directories, default))
            .ReturnsAsync(new[] { sourceDeck });
        
        _deckRepositoryMock
            .Setup(x => x.GetDeck(sourceDeck.DeckId, default))
            .ThrowsAsync(new Exception("Deck not found"));

        // Act
        await _synchronizationService.SynchronizeCardsAsync(directories);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(It.Is<Deck>(d => 
            d.DeckId == sourceDeck.DeckId && 
            d.Cards.Count == 1), default), Times.Once);
    }

    [Fact]
    public async Task SynchronizeCardsAsync_WithExistingDeck_AddsNewCard()
    {
        // Arrange
        var directories = new[] { "test/dir" };
        var existingDeck = CreateTestDeck("TestDeck", "Existing Question?", "Existing Answer!");
        var sourceDeck = CreateTestDeck("TestDeck", "New Question?", "New Answer!");
        
        _cardSourceRepositoryMock
            .Setup(x => x.GetCardsFromDirectories(directories, default))
            .ReturnsAsync(new[] { sourceDeck });
        
        _deckRepositoryMock
            .Setup(x => x.GetDeck(sourceDeck.DeckId, default))
            .ReturnsAsync(existingDeck);

        // Act
        await _synchronizationService.SynchronizeCardsAsync(directories);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(It.Is<Deck>(d => 
            d.DeckId == sourceDeck.DeckId && 
            d.Cards.Count == 2), default), Times.Once);
    }

    [Fact]
    public async Task SynchronizeCardsAsync_WithChangedCard_UpdatesCard()
    {
        // Arrange
        var directories = new[] { "test/dir" };
        var existingDeck = CreateTestDeck("TestDeck", "Question?", "Old Answer!");
        var sourceDeck = CreateTestDeck("TestDeck", "Question?", "New Answer!");
        
        _cardSourceRepositoryMock
            .Setup(x => x.GetCardsFromDirectories(directories, default))
            .ReturnsAsync(new[] { sourceDeck });
        
        _deckRepositoryMock
            .Setup(x => x.GetDeck(sourceDeck.DeckId, default))
            .ReturnsAsync(existingDeck);

        // Act
        await _synchronizationService.SynchronizeCardsAsync(directories);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(It.IsAny<Deck>(), default), Times.Once);
    }

    private static Deck CreateTestDeck(string deckName, string question, string answer)
    {
        return new Deck
        {
            DeckId = DeckId.FromPath([deckName]),
            Cards = new List<Card>
            {
                new QuestionAnswerCard
                {
                    Id = Guid.NewGuid().ToString(),
                    DateModified = DateTimeOffset.UtcNow,
                    Question = question,
                    Answer = answer
                }
            }
        };
    }
}