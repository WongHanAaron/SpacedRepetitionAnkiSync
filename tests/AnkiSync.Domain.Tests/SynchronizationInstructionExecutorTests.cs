using AnkiSync.Application;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnkiSync.Domain.Tests;

public class SynchronizationInstructionExecutorTests
{
    private readonly Mock<IDeckRepository> _deckRepositoryMock;
    private readonly Mock<ILogger<SynchronizationInstructionExecutor>> _loggerMock;
    private readonly SynchronizationInstructionExecutor _executor;

    public SynchronizationInstructionExecutorTests()
    {
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _loggerMock = new Mock<ILogger<SynchronizationInstructionExecutor>>();
        _executor = new SynchronizationInstructionExecutor(
            _deckRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithNullInstructions_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _executor.ExecuteInstructionsAsync(null!));
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithCreateDeckInstruction_CreatesEmptyDeck()
    {
        // Arrange
        var deckId = DeckId.FromPath("Test", "Deck");
        var instruction = new CreateDeckInstruction(deckId);
        var instructions = new[] { instruction };

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => d.DeckId == deckId && d.Cards.Count == 0),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithDeleteDeckInstruction_DeletesDeck()
    {
        // Arrange
        var deckId = DeckId.FromPath("Test", "Deck");
        var instruction = new DeleteDeckInstruction(deckId);
        var instructions = new[] { instruction };

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.DeleteDeckAsync(deckId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithCreateCardInstruction_AddsCardToDeck()
    {
        // Arrange
        var deckId = DeckId.FromPath("Test", "Deck");
        var card = new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.UtcNow,
            Question = "Q",
            Answer = "A"
        };
        var instruction = new CreateCardInstruction(deckId, card);
        var instructions = new[] { instruction };

        var existingDeck = new Deck { DeckId = deckId, Cards = new List<Card>() };
        _deckRepositoryMock.Setup(x => x.GetDeck(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDeck);

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => d.DeckId == deckId && d.Cards.Contains(card)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithCreateCardInstruction_CreatesDeckIfNotExists()
    {
        // Arrange
        var deckId = DeckId.FromPath("Test", "Deck");
        var card = new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.UtcNow,
            Question = "Q",
            Answer = "A"
        };
        var instruction = new CreateCardInstruction(deckId, card);
        var instructions = new[] { instruction };

        _deckRepositoryMock.Setup(x => x.GetDeck(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Deck?)null);

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => d.DeckId == deckId && d.Cards.Contains(card)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithUpdateCardInstruction_UpdatesCardInDeck()
    {
        // Arrange
        var deckId = DeckId.FromPath("Test", "Deck");
        var existingCard = new QuestionAnswerCard
        {
            Id = 1,
            DateModified = DateTimeOffset.UtcNow.AddMinutes(-1),
            Question = "Old Q",
            Answer = "Old A"
        };
        var updatedCard = new QuestionAnswerCard
        {
            Id = 1,
            DateModified = DateTimeOffset.UtcNow,
            Question = "New Q",
            Answer = "New A"
        };
        var instruction = new UpdateCardInstruction(1, updatedCard);
        var instructions = new[] { instruction };

        var existingDeck = new Deck { DeckId = deckId, Cards = new List<Card> { existingCard } };
        _deckRepositoryMock.Setup(x => x.GetDeck(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDeck);
        _deckRepositoryMock.Setup(x => x.GetAllDeckIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { deckId });

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => d.Cards.Any(c => c.Id == 1 && ((QuestionAnswerCard)c).Question == "New Q")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithUpdateCardInstruction_ThrowsIfCardNotFound()
    {
        // Arrange
        var instruction = new UpdateCardInstruction(999, new QuestionAnswerCard
        {
            DateModified = DateTimeOffset.UtcNow
        });
        var instructions = new[] { instruction };

        _deckRepositoryMock.Setup(x => x.GetAllDeckIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<DeckId>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _executor.ExecuteInstructionsAsync(instructions));
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithDeleteCardInstruction_RemovesCardFromDeck()
    {
        // Arrange
        var deckId = DeckId.FromPath("Test", "Deck");
        var card = new QuestionAnswerCard
        {
            Id = 1,
            DateModified = DateTimeOffset.UtcNow,
            Question = "Q",
            Answer = "A"
        };
        var instruction = new DeleteCardInstruction(1);
        var instructions = new[] { instruction };

        var existingDeck = new Deck { DeckId = deckId, Cards = new List<Card> { card } };
        _deckRepositoryMock.Setup(x => x.GetDeck(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDeck);
        _deckRepositoryMock.Setup(x => x.GetAllDeckIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { deckId });

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => !d.Cards.Any(c => c.Id == 1)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithMoveCardInstruction_MovesCardBetweenDecks()
    {
        // Arrange
        var sourceDeckId = DeckId.FromPath("Source", "Deck");
        var targetDeckId = DeckId.FromPath("Target", "Deck");
        var card = new QuestionAnswerCard
        {
            Id = 1,
            DateModified = DateTimeOffset.UtcNow,
            Question = "Q",
            Answer = "A"
        };
        var instruction = new MoveCardInstruction(1, targetDeckId);
        var instructions = new[] { instruction };

        var sourceDeck = new Deck { DeckId = sourceDeckId, Cards = new List<Card> { card } };
        var targetDeck = new Deck { DeckId = targetDeckId, Cards = new List<Card>() };

        _deckRepositoryMock.Setup(x => x.GetAllDeckIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { sourceDeckId, targetDeckId });
        _deckRepositoryMock.Setup(x => x.GetDeck(sourceDeckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceDeck);
        _deckRepositoryMock.Setup(x => x.GetDeck(targetDeckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetDeck);

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => d.DeckId == sourceDeckId && !d.Cards.Any(c => c.Id == 1)),
            It.IsAny<CancellationToken>()), Times.Once);
        _deckRepositoryMock.Verify(x => x.UpsertDeck(
            It.Is<Deck>(d => d.DeckId == targetDeckId && d.Cards.Any(c => c.Id == 1)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithSyncWithAnkiInstruction_SyncsWithAnkiWeb()
    {
        // Arrange
        var instruction = new SyncWithAnkiInstruction();
        var instructions = new[] { instruction };

        // Act
        await _executor.ExecuteInstructionsAsync(instructions);

        // Assert
        _deckRepositoryMock.Verify(x => x.SyncWithAnkiWebAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteInstructionsAsync_WithUnsupportedInstructionType_ThrowsNotSupportedException()
    {
        // Arrange
        var instruction = new TestInstruction();
        var instructions = new[] { instruction };

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            _executor.ExecuteInstructionsAsync(instructions));
    }

    private class TestInstruction : SynchronizationInstruction
    {
        public override SynchronizationInstructionType InstructionType => (SynchronizationInstructionType)999;
    }
}