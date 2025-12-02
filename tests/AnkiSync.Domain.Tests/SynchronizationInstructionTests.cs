using AnkiSync.Domain.Models;
using Xunit;

namespace AnkiSync.Domain.Tests;

public class SynchronizationInstructionTests
{
    [Fact]
    public void SynchronizationInstruction_IsAbstractBaseClass()
    {
        // This test will fail until we create the SynchronizationInstruction base class
        // Arrange & Act & Assert
        Assert.True(typeof(SynchronizationInstruction).IsAbstract);
    }

    [Fact]
    public void CreateDeckInstruction_CanBeCreated()
    {
        // This test will fail until we create the CreateDeckInstruction class
        // Arrange
        var deckId = DeckId.FromPath("TestDeck");

        // Act
        var instruction = new CreateDeckInstruction(deckId);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(deckId, instruction.DeckId);
    }

    [Fact]
    public void DeleteDeckInstruction_CanBeCreated()
    {
        // This test will fail until we create the DeleteDeckInstruction class
        // Arrange
        var deckId = DeckId.FromPath("TestDeck");

        // Act
        var instruction = new DeleteDeckInstruction(deckId);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(deckId, instruction.DeckId);
    }

    [Fact]
    public void CreateCardInstruction_CanBeCreated()
    {
        // This test will fail until we create the CreateCardInstruction class
        // Arrange
        var deckId = DeckId.FromPath("TestDeck");
        var card = new QuestionAnswerCard
        {
            Question = "Question",
            Answer = "Answer",
            DateModified = DateTimeOffset.Now
        };

        // Act
        var instruction = new CreateCardInstruction(deckId, card);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(deckId, instruction.DeckId);
        Assert.Equal(card, instruction.Card);
    }

    [Fact]
    public void UpdateCardInstruction_CanBeCreated()
    {
        // This test will fail until we create the UpdateCardInstruction class
        // Arrange
        var cardId = 123L;
        var card = new QuestionAnswerCard
        {
            Question = "Updated Question",
            Answer = "Updated Answer",
            DateModified = DateTimeOffset.Now
        };

        // Act
        var instruction = new UpdateCardInstruction(cardId, card);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(123L, instruction.CardId);
        Assert.Equal(card, instruction.Card);
    }

    [Fact]
    public void DeleteCardInstruction_CanBeCreated()
    {
        // This test will fail until we create the DeleteCardInstruction class
        // Arrange
        var cardId = 123L;

        // Act
        var instruction = new DeleteCardInstruction(cardId);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(123L, instruction.CardId);
    }

    [Fact]
    public void MoveCardInstruction_CanBeCreated()
    {
        // This test will fail until we create the MoveCardInstruction class
        // Arrange
        var cardId = 123L;
        var targetDeckId = DeckId.FromPath("TargetDeck");

        // Act
        var instruction = new MoveCardInstruction(cardId, targetDeckId);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(123L, instruction.CardId);
        Assert.Equal(targetDeckId, instruction.TargetDeckId);
    }

    [Fact]
    public void SyncWithAnkiInstruction_CanBeCreated()
    {
        // This test will fail until we create the SyncWithAnkiInstruction class
        // Arrange & Act
        var instruction = new SyncWithAnkiInstruction();

        // Assert
        Assert.NotNull(instruction);
    }

    [Fact]
    public void SynchronizationInstructions_HaveUniqueKeys()
    {
        // Arrange
        var createDeck = new CreateDeckInstruction(DeckId.FromPath("TestDeck"));
        var deleteDeck = new DeleteDeckInstruction(DeckId.FromPath("DeleteDeck"));
        var createCard = new CreateCardInstruction(DeckId.FromPath("CreateCardDeck"), new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        });
        var updateCard = new UpdateCardInstruction(123L, new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        });
        var deleteCard = new DeleteCardInstruction(123L);
        var moveCard = new MoveCardInstruction(123L, DeckId.FromPath("TargetDeck"));
        var sync = new SyncWithAnkiInstruction();

        // Act & Assert - Each instruction should have a unique key
        var keys = new[]
        {
            createDeck.GetUniqueKey(),
            deleteDeck.GetUniqueKey(),
            createCard.GetUniqueKey(),
            updateCard.GetUniqueKey(),
            deleteCard.GetUniqueKey(),
            moveCard.GetUniqueKey(),
            sync.GetUniqueKey()
        };

        Assert.Equal(keys.Length, keys.Distinct().Count());
    }

    [Fact]
    public void CreateDeckInstruction_ThrowsForNullDeckId()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateDeckInstruction(null!));
    }

    [Fact]
    public void DeleteDeckInstruction_ThrowsForNullDeckId()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DeleteDeckInstruction(null!));
    }

    [Fact]
    public void CreateCardInstruction_ThrowsForNullCard()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateCardInstruction(DeckId.FromPath("TestDeck"), null!));
    }

    [Fact]
    public void UpdateCardInstruction_ThrowsForNullCard()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UpdateCardInstruction(123L, null!));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void UpdateCardInstruction_ThrowsForInvalidCardId(long invalidCardId)
    {
        // Arrange
        var card = new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new UpdateCardInstruction(invalidCardId, card));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void DeleteCardInstruction_ThrowsForInvalidCardId(long invalidCardId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DeleteCardInstruction(invalidCardId));
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    public void MoveCardInstruction_ThrowsForInvalidCardId(long invalidCardId)
    {
        // Arrange
        var targetDeckId = DeckId.FromPath("TargetDeck");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new MoveCardInstruction(invalidCardId, targetDeckId));
    }
}