using AnkiSync.Application;
using AnkiSync.Domain;
using AnkiSync.Domain.Extensions;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnkiSync.Application.Tests;

/// <summary>
/// Unit tests for CardSynchronizationService instruction accumulation
/// </summary>
public class CardSynchronizationServiceInstructionTests
{
    private readonly Mock<ICardSourceRepository> _cardSourceRepositoryMock;
    private readonly Mock<IDeckRepository> _deckRepositoryMock;
    private readonly Mock<ILogger<CardSynchronizationService>> _loggerMock;
    private readonly ICardEqualityChecker _cardEqualityChecker;
    private readonly CardSynchronizationService _service;

    public CardSynchronizationServiceInstructionTests()
    {
        _cardSourceRepositoryMock = new Mock<ICardSourceRepository>();
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _loggerMock = new Mock<ILogger<CardSynchronizationService>>();
        _cardEqualityChecker = new ExactMatchEqualityChecker();
        _service = new CardSynchronizationService(
            _cardSourceRepositoryMock.Object,
            _deckRepositoryMock.Object,
            _cardEqualityChecker,
            new ExactMatchDeckIdEqualityChecker(),
            _loggerMock.Object);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithNullSourceDecks_ThrowsArgumentNullException()
    {
        // Arrange
        var existingDecks = new List<Deck>();

        // Act
        Func<Task> act = async () => await _service.AccumulateInstructionsAsync(null!, existingDecks);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("sourceDecks");
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithNullExistingDecks_ThrowsArgumentNullException()
    {
        // Arrange
        var sourceDecks = new List<Deck>();

        // Act
        Func<Task> act = async () => await _service.AccumulateInstructionsAsync(sourceDecks, null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("existingAnkiDecks");
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithEmptyLists_ReturnsSyncInstruction()
    {
        // Arrange
        var sourceDecks = new List<Deck>();
        var existingDecks = new List<Deck>();

        // Act
        var result = await _service.AccumulateInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().ContainSingle(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithSourceDecksAndEmptyExistingDecks_ReturnsCreateInstructions()
    {
        // Arrange
        var sourceDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("TestDeck"),
                Cards = new List<Card>
                {
                    new QuestionAnswerCard { Question = "Q1", Answer = "A1", DateModified = DateTimeOffset.Now }
                }
            }
        };
        var existingDecks = new List<Deck>();

        // Act
        var result = await _service.AccumulateInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // CreateDeck, CreateCard, SyncWithAnki
        result.Should().Contain(instruction => instruction is CreateDeckInstruction);
        result.Should().Contain(instruction => instruction is CreateCardInstruction);
        result.Should().Contain(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithCancellationToken_RespectsCancellation()
    {
        // Arrange
        var sourceDecks = new List<Deck>();
        var existingDecks = new List<Deck>();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.AccumulateInstructionsAsync(sourceDecks, existingDecks, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}