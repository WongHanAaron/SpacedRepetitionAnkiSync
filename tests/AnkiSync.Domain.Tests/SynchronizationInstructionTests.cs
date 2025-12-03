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
        var existingCard = new QuestionAnswerCard
        {
            Question = "Existing Question",
            Answer = "Existing Answer",
            DateModified = DateTimeOffset.Now
        };

        var card = new QuestionAnswerCard
        {
            Question = "Updated Question",
            Answer = "Updated Answer",
            DateModified = DateTimeOffset.Now
        };

        // Act
        var instruction = new UpdateCardInstruction(existingCard, card);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(existingCard, instruction.ExistingCard);
        Assert.Equal(card, instruction.Card);
    }

    [Fact]
    public void DeleteCardInstruction_CanBeCreated()
    {
        // Arrange
        var card = new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        };

        // Act
        var instruction = new DeleteCardInstruction(card);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(card, instruction.Card);
    }

    [Fact]
    public void MoveCardInstruction_CanBeCreated()
    {
        // This test will fail until we create the MoveCardInstruction class
        // Arrange
        var card = new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        };
        var targetDeckId = DeckId.FromPath("TargetDeck");

        // Act
        var instruction = new MoveCardInstruction(card, targetDeckId);

        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(card, instruction.Card);
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
        var existingForUpdate = new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        };
        var newForUpdate = new QuestionAnswerCard
        {
            Question = "Q",
            Answer = "A",
            DateModified = DateTimeOffset.Now
        };

        var updateCard = new UpdateCardInstruction(existingForUpdate, newForUpdate);
        var deleteCard = new DeleteCardInstruction(new QuestionAnswerCard { Question = "Q", Answer = "A", DateModified = DateTimeOffset.Now });
        var moveCard = new MoveCardInstruction(new QuestionAnswerCard { Question = "Q", Answer = "A", DateModified = DateTimeOffset.Now }, DeckId.FromPath("TargetDeck"));
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
    public void UpdateCardInstruction_ThrowsForNullArguments()
    {
        var card = new QuestionAnswerCard { Question = "Q", Answer = "A", DateModified = DateTimeOffset.Now };

        Assert.Throws<ArgumentNullException>(() => new UpdateCardInstruction(null!, card));
        Assert.Throws<ArgumentNullException>(() => new UpdateCardInstruction(card, null!));
    }

    [Fact]
    public void DeleteCardInstruction_ThrowsForNullArgument()
    {
        Assert.Throws<ArgumentNullException>(() => new DeleteCardInstruction(null!));
    }

    [Fact]
    public void MoveCardInstruction_ThrowsForNullArguments()
    {
        var card = new QuestionAnswerCard { Question = "Q", Answer = "A", DateModified = DateTimeOffset.Now };
        Assert.Throws<ArgumentNullException>(() => new MoveCardInstruction(null!, DeckId.FromPath("TargetDeck")));
        Assert.Throws<ArgumentNullException>(() => new MoveCardInstruction(card, null!));
    }
}