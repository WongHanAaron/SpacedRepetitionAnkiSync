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
        Func<Task> act = async () => await _service.AccumulateSyncInstructionsAsync(null!, existingDecks);

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
        Func<Task> act = async () => await _service.AccumulateSyncInstructionsAsync(sourceDecks, null!);

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
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

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
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

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
        Func<Task> act = async () => await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithDefaultDeckInExistingDecks_DoesNotDeleteDefaultDeck()
    {
        // Arrange
        var sourceDecks = new List<Deck>();
        var existingDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("Default"),
                Cards = new List<Card>()
            }
        };

        // Act
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1); // Only SyncWithAnki instruction, no DeleteDeck
        result.Should().ContainSingle(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithParentDeckHavingChildren_DoesNotDeleteParentDeck()
    {
        // Arrange
        var sourceDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("Parent", "Child"),
                Cards = new List<Card>()
            }
        };
        var existingDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("Parent"),
                Cards = new List<Card>()
            },
            new Deck
            {
                DeckId = DeckId.FromPath("Parent", "Child"),
                Cards = new List<Card>()
            }
        };

        // Act
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1); // Only SyncWithAnki instruction, no DeleteDeck instructions
        result.Should().ContainSingle(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithUnrelatedExistingDecks_DeletesThem()
    {
        // Arrange
        var sourceDecks = new List<Deck>();
        var existingDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("UnrelatedDeck"),
                Cards = new List<Card>()
            }
        };

        // Act
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // DeleteDeck and SyncWithAnki
        result.Should().Contain(instruction => instruction is DeleteDeckInstruction);
        result.Should().Contain(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithCardInDifferentDeck_MovesCardToCorrectDeck()
    {
        // Arrange - Card exists in "OldDeck" but source has it in "NewDeck"
        var sourceCard = new QuestionAnswerCard 
        { 
            Question = "Test Question?", 
            Answer = "Test Answer",
            DateModified = DateTimeOffset.Now 
        };
        
        var sourceDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("NewDeck"),
                Cards = new List<Card> { sourceCard }
            }
        };
        
        var existingCard = new QuestionAnswerCard 
        { 
            Question = "Test Question?", 
            Answer = "Test Answer",
            DateModified = DateTimeOffset.Now // Same content and date, so no update needed
        };
        
        var existingDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("OldDeck"),
                Cards = new List<Card> { existingCard }
            },
            new Deck
            {
                DeckId = DeckId.FromPath("NewDeck"),
                Cards = new List<Card>()
            }
        };

        // Act
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // DeleteDeck (OldDeck), MoveCard, SyncWithAnki (no UpdateCard since content is same)
        
        var deleteInstructions = result.OfType<DeleteDeckInstruction>().ToList();
        deleteInstructions.Should().HaveCount(1);
        deleteInstructions[0].DeckId.Name.Should().Be("OldDeck");
        deleteInstructions[0].DeckId.Parents.Should().BeEmpty();
        
        var moveInstructions = result.OfType<MoveCardInstruction>().ToList();
        moveInstructions.Should().HaveCount(1);
        moveInstructions[0].Card.Should().Be(existingCard);
        moveInstructions[0].TargetDeckId.Name.Should().Be("NewDeck");
        moveInstructions[0].TargetDeckId.Parents.Should().BeEmpty();
        
        result.Should().Contain(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithCardInDifferentDeckAndContentChanged_UpdatesAndMovesCard()
    {
        // Arrange - Card exists in "OldDeck" but source has it in "NewDeck" with different content
        var sourceCard = new QuestionAnswerCard 
        { 
            Question = "Test Question?", 
            Answer = "Updated Answer", // Changed content
            DateModified = DateTimeOffset.Now 
        };
        
        var sourceDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("NewDeck"),
                Cards = new List<Card> { sourceCard }
            }
        };
        
        var existingCard = new QuestionAnswerCard 
        { 
            Question = "Test Question?", 
            Answer = "Old Answer", // Different content
            DateModified = DateTimeOffset.Now.AddMinutes(-1) 
        };
        
        var existingDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("OldDeck"),
                Cards = new List<Card> { existingCard }
            },
            new Deck
            {
                DeckId = DeckId.FromPath("NewDeck"),
                Cards = new List<Card>()
            }
        };

        // Act
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4); // DeleteDeck (OldDeck), UpdateCard, MoveCard, SyncWithAnki
        
        var deleteInstructions = result.OfType<DeleteDeckInstruction>().ToList();
        deleteInstructions.Should().HaveCount(1);
        deleteInstructions[0].DeckId.Name.Should().Be("OldDeck");
        deleteInstructions[0].DeckId.Parents.Should().BeEmpty();
        
        var updateInstructions = result.OfType<UpdateCardInstruction>().ToList();
        updateInstructions.Should().HaveCount(1);
        updateInstructions[0].ExistingCard.Should().Be(existingCard);
        updateInstructions[0].Card.Should().BeOfType<QuestionAnswerCard>();
        ((QuestionAnswerCard)updateInstructions[0].Card).Answer.Should().Be("Updated Answer");
        
        var moveInstructions = result.OfType<MoveCardInstruction>().ToList();
        moveInstructions.Should().HaveCount(1);
        moveInstructions[0].Card.Should().Be(existingCard);
        moveInstructions[0].TargetDeckId.Name.Should().Be("NewDeck");
        moveInstructions[0].TargetDeckId.Parents.Should().BeEmpty();
        
        result.Should().Contain(instruction => instruction is SyncWithAnkiInstruction);
    }

    [Fact]
    public async Task AccumulateInstructionsAsync_WithCardInNestedChildDeck_MovesCardToParentDeck()
    {
        // Arrange - Card exists in nested "Parent::Child::Grandchild" but source has it in "Parent"
        var sourceCard = new QuestionAnswerCard 
        { 
            Question = "Test Question?", 
            Answer = "Test Answer",
            DateModified = DateTimeOffset.Now 
        };
        
        var sourceDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("Parent"),
                Cards = new List<Card> { sourceCard }
            }
        };
        
        var existingCard = new QuestionAnswerCard 
        { 
            Question = "Test Question?", 
            Answer = "Test Answer",
            DateModified = DateTimeOffset.Now // Same content, so no update needed
        };
        
        var existingDecks = new List<Deck>
        {
            new Deck
            {
                DeckId = DeckId.FromPath("Parent"),
                Cards = new List<Card>()
            },
            new Deck
            {
                DeckId = DeckId.FromPath("Parent", "Child"),
                Cards = new List<Card>()
            },
            new Deck
            {
                DeckId = DeckId.FromPath("Parent", "Child", "Grandchild"),
                Cards = new List<Card> { existingCard }
            }
        };

        // Act
        var result = await _service.AccumulateSyncInstructionsAsync(sourceDecks, existingDecks);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // DeleteDeck (Grandchild only, since Child has Grandchild as child), MoveCard, SyncWithAnki
        
        var deleteInstructions = result.OfType<DeleteDeckInstruction>().ToList();
        deleteInstructions.Should().HaveCount(1);
        
        // Check that only Grandchild deck is marked for deletion (Child is protected because it has Grandchild as child)
        var grandchildDelete = deleteInstructions.FirstOrDefault(d => d.DeckId.Name == "Grandchild");
        grandchildDelete.Should().NotBeNull();
        grandchildDelete?.DeckId.Parents.Should().BeEquivalentTo(new[] { "Parent", "Child" });
        
        var moveInstructions = result.OfType<MoveCardInstruction>().ToList();
        moveInstructions.Should().HaveCount(1);
        moveInstructions[0].Card.Should().Be(existingCard);
        moveInstructions[0].TargetDeckId.Name.Should().Be("Parent");
        moveInstructions[0].TargetDeckId.Parents.Should().BeEmpty();
        
        result.Should().Contain(instruction => instruction is SyncWithAnkiInstruction);
    }
}