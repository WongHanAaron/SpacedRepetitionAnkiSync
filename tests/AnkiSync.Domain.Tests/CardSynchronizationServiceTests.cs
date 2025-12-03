using AnkiSync.Application;
using AnkiSync.Domain;
using AnkiSync.Domain.Extensions;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
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
    private readonly ICardEqualityChecker _cardEqualityChecker;
    private readonly CardSynchronizationService _synchronizationService;

    public CardSynchronizationServiceTests()
    {
        _cardSourceRepositoryMock = new Mock<ICardSourceRepository>();
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _loggerMock = new Mock<ILogger<CardSynchronizationService>>();
        _cardEqualityChecker = new ExactMatchEqualityChecker();
        _synchronizationService = new CardSynchronizationService(
            _cardSourceRepositoryMock.Object,
            _deckRepositoryMock.Object,
            _cardEqualityChecker,
            new ExactMatchDeckIdEqualityChecker(),
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
            .Setup(x => x.GetAllDecksAsync(default))
            .ReturnsAsync(Array.Empty<Deck>());

        // Act
        await _synchronizationService.SynchronizeCardsAsync(directories);

        // Assert
        _deckRepositoryMock.Verify(x => x.ExecuteInstructionsAsync(
            It.Is<IReadOnlyList<SynchronizationInstruction>>(instructions =>
                instructions.Count == 3 &&
                instructions[0].InstructionType == SynchronizationInstructionType.CreateDeck &&
                ((CreateDeckInstruction)instructions[0]).DeckId == sourceDeck.DeckId &&
                instructions[1].InstructionType == SynchronizationInstructionType.CreateCard &&
                ((CreateCardInstruction)instructions[1]).DeckId == sourceDeck.DeckId &&
                instructions[2].InstructionType == SynchronizationInstructionType.SyncWithAnki), default), Times.Once);
    }

    [Fact]
    public async Task SynchronizeCardsAsync_WithExistingDeck_AddsNewCard()
    {
        // Arrange
        var directories = new[] { "test/dir" };
        var deckId = DeckId.FromPath("TestDeck");
        var dateModified = DateTimeOffset.UtcNow;
        var existingDeck = CreateTestDeck(deckId, dateModified, "Existing Question?", "Existing Answer!");
        existingDeck.Cards[0].Id = 1; // Simulate existing card with ID
        var sourceDeck = CreateTestDeck(deckId, dateModified, "Existing Question?", "Existing Answer!", "New Question?", "New Answer!");
        
        _cardSourceRepositoryMock
            .Setup(x => x.GetCardsFromDirectories(directories, default))
            .ReturnsAsync(new[] { sourceDeck });
        
        _deckRepositoryMock
            .Setup(x => x.GetAllDecksAsync(default))
            .ReturnsAsync(new[] { existingDeck });

        // Act
        await _synchronizationService.SynchronizeCardsAsync(directories);

        // Assert
        _deckRepositoryMock.Verify(x => x.ExecuteInstructionsAsync(
            It.Is<IReadOnlyList<SynchronizationInstruction>>(instructions =>
                instructions.Count == 2 &&
                instructions[0].InstructionType == SynchronizationInstructionType.CreateCard &&
                ((CreateCardInstruction)instructions[0]).DeckId == sourceDeck.DeckId &&
                instructions[1].InstructionType == SynchronizationInstructionType.SyncWithAnki), default), Times.Once);
    }

    [Fact]
    public async Task SynchronizeCardsAsync_WithChangedCard_UpdatesCard()
    {
        // Arrange
        var directories = new[] { "test/dir" };
        var deckId = DeckId.FromPath("TestDeck");
        var existingDeck = CreateTestDeck(deckId, "Question?", "Old Answer!");
        existingDeck.Cards[0].Id = 1; // Simulate existing card with ID
        var sourceDeck = CreateTestDeck(deckId, "Question?", "New Answer!");
        
        _cardSourceRepositoryMock
            .Setup(x => x.GetCardsFromDirectories(directories, default))
            .ReturnsAsync(new[] { sourceDeck });
        
        _deckRepositoryMock
            .Setup(x => x.GetAllDecksAsync(default))
            .ReturnsAsync(new[] { existingDeck });

        // Act
        await _synchronizationService.SynchronizeCardsAsync(directories);

        // Assert
        _deckRepositoryMock.Verify(x => x.ExecuteInstructionsAsync(
            It.Is<IReadOnlyList<SynchronizationInstruction>>(instructions =>
                instructions.Count == 2 &&
                instructions[0].InstructionType == SynchronizationInstructionType.UpdateCard &&
                ((UpdateCardInstruction)instructions[0]).CardId == 1 &&
                instructions[1].InstructionType == SynchronizationInstructionType.SyncWithAnki), default), Times.Once);
    }

    private static Deck CreateTestDeck(string deckName, params string[] questionAnswerPairs)
    {
        return CreateTestDeck(DeckId.FromPath(deckName), questionAnswerPairs);
    }

    private static Deck CreateTestDeck(DeckId deckId, params string[] questionAnswerPairs)
    {
        var dateModified = DateTimeOffset.UtcNow;
        return CreateTestDeck(deckId, dateModified, questionAnswerPairs);
    }

    private static Deck CreateTestDeck(DeckId deckId, DateTimeOffset dateModified, params string[] questionAnswerPairs)
    {
        var cards = new List<Card>();
        for (int i = 0; i < questionAnswerPairs.Length; i += 2)
        {
            cards.Add(new QuestionAnswerCard
            {
                DateModified = dateModified,
                Question = questionAnswerPairs[i],
                Answer = questionAnswerPairs[i + 1]
            });
        }

        return new Deck
        {
            DeckId = deckId,
            Cards = cards
        };
    }
}