using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Application;
using AnkiSync.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AnkiSync.IntegrationTests;

/// <summary>
/// Integration tests for card synchronization that mock AnkiConnect and file system.
/// Tests the complete flow from file parsing to deck synchronization.
/// </summary>
[Collection("Synchronization Integration Tests")]
public class SynchronizationIntegrationTests
{
    [Fact]
    public async Task SynchronizeCards_WithNewMarkdownFile_CreatesNewDeckWithCards()
    {
        // Arrange - Set up mock file system with markdown file
        var mockFileSystem = new MockFileSystem();
        var testDir = "/testdir";
        mockFileSystem.AddDirectory(testDir);
        var markdownFile = $"{testDir}/test_deck.md";
        var fileContent = """
#test_deck
Test Question 1?::This is the answer to question 1.
Test Question 2?::This is the answer to question 2.
""";
        mockFileSystem.AddFile(markdownFile, new MockFileData(fileContent));

        // Create a list to capture all decks that are upserted
        var capturedDecks = new List<Deck>();
        var deckRepositoryMock = new Mock<IDeckRepository>();
        
        // Mock GetDeck to always return null (deck doesn't exist)
        deckRepositoryMock
            .Setup(x => x.GetDeck(It.IsAny<DeckId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null!); // Use null! to indicate we're intentionally returning null for a non-nullable type
            
        // Mock UpsertDeck to capture the deck
        deckRepositoryMock
            .Setup(x => x.UpsertDeck(It.IsAny<Deck>(), It.IsAny<CancellationToken>()))
            .Callback<Deck, CancellationToken>((deck, _) => 
            {
                capturedDecks.Add(deck);
            });

        // Create services
        var fileParser = new FileParser();
        var cardExtractor = new CardExtractor(NullLogger<CardExtractor>.Instance);
        var deckInferencer = new DeckInferencer(mockFileSystem);
        var logger = NullLogger<SpacedRepetitionNotesRepository>.Instance;
        var cardSourceRepository = new SpacedRepetitionNotesRepository(fileParser, cardExtractor, deckInferencer, mockFileSystem, logger);
        var syncLogger = NullLogger<CardSynchronizationService>.Instance;
        var synchronizationService = new CardSynchronizationService(cardSourceRepository, deckRepositoryMock.Object, syncLogger);

        // Act
        await synchronizationService.SynchronizeCardsAsync(new[] { testDir });

        // Assert - Verify the deck was created with the correct cards
        capturedDecks.Should().NotBeEmpty();
        
        // Combine all cards from all decks
        var allCards = capturedDecks.SelectMany(d => d.Cards ?? Enumerable.Empty<Card>()).ToList();
        
        // Verify we have exactly 2 cards across all decks
        allCards.Should().HaveCount(2, "because we expect 2 question-answer cards");
        
        // Verify both cards are of type QuestionAnswerCard
        var qaCards = allCards.OfType<QuestionAnswerCard>().ToList();
        qaCards.Should().HaveCount(2, "because we expect 2 question-answer cards");
        
        // Verify the content of the cards
        var card1 = qaCards.FirstOrDefault(c => c.Question == "Test Question 1?");
        card1.Should().NotBeNull();
        card1?.Answer.Should().Be("This is the answer to question 1.");
        
        var card2 = qaCards.FirstOrDefault(c => c.Question == "Test Question 2?");
        card2.Should().NotBeNull();
        card2?.Answer.Should().Be("This is the answer to question 2.");
    }

    [Fact]
    public async Task SynchronizeCards_WithExistingDeck_AddsNewCards()
    {
        // Arrange - Set up mock file system with markdown file
        var mockFileSystem = new MockFileSystem();
        var testDir = "/testdir2";
        mockFileSystem.AddDirectory(testDir);
        var markdownFile = $"{testDir}/existing_deck.md";
        mockFileSystem.AddFile(markdownFile, """
#test
Existing Question?::Existing Answer.
New Question?::New Answer.
""");

        var deckRepositoryMock = new Mock<IDeckRepository>();
        Deck? capturedDeck = null;
        deckRepositoryMock
            .Setup(x => x.UpsertDeck(It.IsAny<Deck>(), default))
            .Callback<Deck, CancellationToken>((deck, _) => capturedDeck = deck);

        // Mock existing deck with one card
        var existingDeck = new Deck
        {
            DeckId = DeckId.FromPath(["test"]),
            Cards = new List<Card>
            {
                new QuestionAnswerCard
                {
                    DateModified = DateTimeOffset.Parse("2025-11-28T00:00:00Z"),
                    Question = "Existing Question?",
                    Answer = "Existing Answer."
                }
            }
        };

        deckRepositoryMock
            .Setup(x => x.GetDeck(It.Is<DeckId>(id => id.Name == "existing_deck"), default))
            .ReturnsAsync(existingDeck);

        // Create services manually
        var fileParser = new FileParser();
        var cardExtractor = new CardExtractor(NullLogger<CardExtractor>.Instance);
        
        var deckInferencer = new DeckInferencer(mockFileSystem);
        var logger = NullLogger<SpacedRepetitionNotesRepository>.Instance;
        var cardSourceRepository = new SpacedRepetitionNotesRepository(fileParser, cardExtractor, deckInferencer, mockFileSystem, logger);
        var syncLogger = NullLogger<CardSynchronizationService>.Instance;
        var synchronizationService = new CardSynchronizationService(cardSourceRepository, deckRepositoryMock.Object, syncLogger);

        // Act
        await synchronizationService.SynchronizeCardsAsync(new[] { testDir });

        // Assert - Should have both existing and new cards
        capturedDeck.Should().NotBeNull();
        if (capturedDeck != null)
        {
            capturedDeck.Cards.Should().NotBeNull();
            capturedDeck.Cards.Should().HaveCount(2);
            
            // Check that existing card is preserved
            var existingCard = capturedDeck.Cards.FirstOrDefault(c => c is QuestionAnswerCard qa && qa.Question == "Existing Question?" && qa.Answer == "Existing Answer.");
            existingCard.Should().NotBeNull();
            
            var qaCards = capturedDeck.Cards.OfType<QuestionAnswerCard>().ToList();
            qaCards.Should().Contain(c => c.Question == "New Question?" && c.Answer == "New Answer."); // New card added
        }
    }

    [Fact(Skip = "Not functional for now")]
    public async Task SynchronizeCards_WithUpdatedCard_UpdatesExistingCard()
    {
        // Arrange - Set up mock file system with markdown file
        var mockFileSystem = new MockFileSystem();
        var testDir = "/testdir3";
        mockFileSystem.AddDirectory(testDir);
        var markdownFile = $"{testDir}/update_deck.md";
        mockFileSystem.AddFile(markdownFile, """
#test
Question?::Updated Answer.
""");

        var deckRepositoryMock = new Mock<IDeckRepository>();
        Deck? capturedDeck = null;
        deckRepositoryMock
            .Setup(x => x.UpsertDeck(It.IsAny<Deck>(), default))
            .Callback<Deck, CancellationToken>((deck, _) => capturedDeck = deck);

        // Mock existing deck with outdated card
        var existingDeck = new Deck
        {
            DeckId = DeckId.FromPath(["update_deck"]),
            Cards = new List<Card>
            {
                new QuestionAnswerCard
                {
                    DateModified = DateTimeOffset.Parse("2025-11-27T00:00:00Z"), // Older
                    Question = "Question?",
                    Answer = "Old Answer."
                }
            }
        };

        deckRepositoryMock
            .Setup(x => x.GetDeck(It.Is<DeckId>(id => id.Name == "update_deck"), default))
            .ReturnsAsync(existingDeck);

        // Create services manually
        var fileParser = new FileParser();
        var cardExtractor = new CardExtractor(NullLogger<CardExtractor>.Instance);
        
        var deckInferencer = new DeckInferencer(mockFileSystem);
        var logger = NullLogger<SpacedRepetitionNotesRepository>.Instance;
        var cardSourceRepository = new SpacedRepetitionNotesRepository(fileParser, cardExtractor, deckInferencer, mockFileSystem, logger);
        var syncLogger = NullLogger<CardSynchronizationService>.Instance;
        var synchronizationService = new CardSynchronizationService(cardSourceRepository, deckRepositoryMock.Object, syncLogger);

        // Act
        await synchronizationService.SynchronizeCardsAsync(new[] { testDir });

        // Assert - Card should be updated with new content
        capturedDeck.Should().NotBeNull();
        if (capturedDeck != null)
        {
            capturedDeck.Cards.Should().NotBeNull();
            capturedDeck.Cards.Should().HaveCount(1);
            
            if (capturedDeck.Cards.Count > 0)
            {
                var qaCard = capturedDeck.Cards[0] as QuestionAnswerCard;
                qaCard.Should().NotBeNull();
                if (qaCard != null)
                {
                    qaCard.Question.Should().Be("Question?");
                    qaCard.Answer.Should().Be("Updated Answer."); // Updated content
                }
            }
        }
    }

    [Fact]
    public async Task SynchronizeCards_With_Different_Directories_Creates_Same_Deck()
    {
        // Arrange - Set up mock file system with nested directories
        var mockFileSystem = new MockFileSystem();
        var testDir = "/testdir4";
        var subDir = $"{testDir}/subdir";
        mockFileSystem.AddDirectory(testDir);
        mockFileSystem.AddDirectory(subDir);
        mockFileSystem.AddFile($"{testDir}/deck1.md", """
#test
Q1?::A1.
""");
        mockFileSystem.AddFile($"{subDir}/deck2.md", """
#test
Q2?::A2.
""");

        var deckRepositoryMock = new Mock<IDeckRepository>();
        var capturedDecks = new List<Deck>();
        deckRepositoryMock
            .Setup(x => x.UpsertDeck(It.IsAny<Deck>(), default))
            .Callback<Deck, CancellationToken>((deck, _) => capturedDecks.Add(deck));

        // Mock deck repository - decks don't exist
        deckRepositoryMock
            .Setup(x => x.GetDeck(It.IsAny<DeckId>(), default)).ReturnsAsync((Deck?)null);

        // Create services manually
        var fileParser = new FileParser();
        var cardExtractor = new CardExtractor(NullLogger<CardExtractor>.Instance);
        
        var deckInferencer = new DeckInferencer(mockFileSystem);
        var logger = NullLogger<SpacedRepetitionNotesRepository>.Instance;
        var cardSourceRepository = new SpacedRepetitionNotesRepository(fileParser, cardExtractor, deckInferencer, mockFileSystem, logger);
        var syncLogger = NullLogger<CardSynchronizationService>.Instance;
        var synchronizationService = new CardSynchronizationService(cardSourceRepository, deckRepositoryMock.Object, syncLogger);

        // Act
        await synchronizationService.SynchronizeCardsAsync(new[] { testDir });

        // Assert - Both decks should be created
        capturedDecks.Should().HaveCount(1);
        capturedDecks.Should().Contain(d => d.DeckId.Name == "test");
    }

    [Fact]
    public async Task SynchronizeCards_WithClozeCards_ParsesClozeFormatCorrectly()
    {
        // Arrange - Set up mock file system with cloze cards
        var mockFileSystem = new MockFileSystem();
        var testDir = "/testdir5";
        mockFileSystem.AddDirectory(testDir);
        var markdownFile = $"{testDir}/cloze_deck.md";
        mockFileSystem.AddFile(markdownFile, """
#cloze
The capital of {{c1::France}} is {{c2::Paris}}.

#cloze #geography
The {{c3::Nile}} is the longest {{c4::river}} in the world.
""");

        var deckRepositoryMock = new Mock<IDeckRepository>();
        Deck? capturedDeck = null;
        deckRepositoryMock
            .Setup(x => x.UpsertDeck(It.IsAny<Deck>(), default))
            .Callback<Deck, CancellationToken>((deck, _) => capturedDeck = deck);

        // Mock deck repository - deck doesn't exist
        deckRepositoryMock
            .Setup(x => x.GetDeck(It.IsAny<DeckId>(), default)).ReturnsAsync((Deck?)null);  

        // Create services manually
        var fileParser = new FileParser();
        var cardExtractor = new CardExtractor(NullLogger<CardExtractor>.Instance);
        
        var deckInferencer = new DeckInferencer(mockFileSystem);
        var logger = NullLogger<SpacedRepetitionNotesRepository>.Instance;
        var cardSourceRepository = new SpacedRepetitionNotesRepository(fileParser, cardExtractor, deckInferencer, mockFileSystem, logger);
        var syncLogger = NullLogger<CardSynchronizationService>.Instance;
        var synchronizationService = new CardSynchronizationService(cardSourceRepository, deckRepositoryMock.Object, syncLogger);

        // Act
        await synchronizationService.SynchronizeCardsAsync(new[] { testDir });

        // Assert - Should create deck with cloze cards
        capturedDeck.Should().NotBeNull();
        if (capturedDeck != null)
        {
            capturedDeck.DeckId.Should().NotBeNull();
            capturedDeck.DeckId.Name.Should().Be("cloze");
            capturedDeck.Cards.Should().NotBeNull();
            capturedDeck.Cards.Should().HaveCount(2);
            capturedDeck.Cards.Should().AllBeOfType<ClozeCard>();
            
            var clozeCards = capturedDeck.Cards.Cast<ClozeCard>().ToList();
            clozeCards.Should().Contain(c => 
                c.Text != null && c.Text.Contains("The capital of {c1} is {c2}.") 
                && c.Answers.ContainsKey("c1") && c.Answers.ContainsKey("c2"));
            clozeCards.Should().Contain(c => 
                c.Text != null && c.Text.Contains("The {c3} is the longest {c4} in the world.")
                && c.Answers.ContainsKey("c3") && c.Answers.ContainsKey("c4"));
        }
    }
}
