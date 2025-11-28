using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AnkiSync.Adapter.AnkiConnect.Tests;

public class DeckRepositoryTests
{
    private readonly Mock<IAnkiService> _ankiServiceMock;
    private readonly Mock<ILogger<DeckRepository>> _loggerMock;
    private readonly DeckRepository _deckRepository;

    public DeckRepositoryTests()
    {
        _ankiServiceMock = new Mock<IAnkiService>();
        _loggerMock = new Mock<ILogger<DeckRepository>>();
        _deckRepository = new DeckRepository(_ankiServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetDeck_ShouldThrowArgumentNullException_WhenDeckIdIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _deckRepository.GetDeck(null!));
    }

    [Fact]
    public async Task GetDeck_ShouldReturnEmptyDeck_WhenNoNotesFound()
    {
        // Arrange
        var deckId = DeckIdExtensions.FromAnkiDeckName("TestDeck");
        var findNotesResponse = new FindNotesResponse { Result = new List<long>() };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestDeck");
        result.Cards.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDeck_ShouldReturnDeckWithCards_WhenNotesFound()
    {
        // Arrange
        var deckId = DeckIdExtensions.FromAnkiDeckName("TestDeck");
        var noteIds = new List<long> { 123, 456 };
        var findNotesResponse = new FindNotesResponse { Result = noteIds };

        var notesInfo = new List<NoteInfo>
        {
            new NoteInfo
            {
                NoteId = 123,
                ModelName = "Basic",
                ModificationTimestamp = 1000000000,
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Front"] = new NoteFieldInfo { Value = "Question 1?" },
                    ["Back"] = new NoteFieldInfo { Value = "Answer 1!" }
                }
            },
            new NoteInfo
            {
                NoteId = 456,
                ModelName = "Basic",
                ModificationTimestamp = 1000000001,
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Front"] = new NoteFieldInfo { Value = "Question 2?" },
                    ["Back"] = new NoteFieldInfo { Value = "Answer 2!" }
                }
            }
        };
        var notesInfoResponse = new NotesInfoResponse { Result = notesInfo };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        _ankiServiceMock
            .Setup(x => x.NotesInfoAsync(It.IsAny<NotesInfoRequestDto>(), default))
            .ReturnsAsync(notesInfoResponse);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestDeck");
        result.Cards.Should().HaveCount(2);

        var firstCard = result.Cards[0] as QuestionAnswerCard;
        firstCard.Should().NotBeNull();
        firstCard!.Id.Should().Be("123");
        firstCard.Question.Should().Be("Question 1?");
        firstCard.Answer.Should().Be("Answer 1!");
    }

    [Fact]
    public async Task GetDeck_ShouldHandleClozeCards()
    {
        // Arrange
        var deckId = DeckIdExtensions.FromAnkiDeckName("TestDeck");
        var noteIds = new List<long> { 789 };
        var findNotesResponse = new FindNotesResponse { Result = noteIds };

        var notesInfo = new List<NoteInfo>
        {
            new NoteInfo
            {
                NoteId = 789,
                ModelName = "Cloze",
                ModificationTimestamp = 1000000002,
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Text"] = new NoteFieldInfo { Value = "This is a {{c1::cloze}} test." }
                }
            }
        };
        var notesInfoResponse = new NotesInfoResponse { Result = notesInfo };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        _ankiServiceMock
            .Setup(x => x.NotesInfoAsync(It.IsAny<NotesInfoRequestDto>(), default))
            .ReturnsAsync(notesInfoResponse);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull();
        result.Cards.Should().HaveCount(1);

        var clozeCard = result.Cards[0] as ClozeCard;
        clozeCard.Should().NotBeNull();
        clozeCard!.Id.Should().Be("789");
        clozeCard.Text.Should().Be("This is a {answer1} test.");
        clozeCard.Answers.Should().ContainKey("answer1");
        clozeCard.Answers["answer1"].Should().Be("cloze");
    }

    [Fact]
    public async Task GetDeck_ShouldSkipNotesWithMissingFields()
    {
        // Arrange
        var deckId = DeckIdExtensions.FromAnkiDeckName("TestDeck");
        var noteIds = new List<long> { 123, 456 };
        var findNotesResponse = new FindNotesResponse { Result = noteIds };

        var notesInfo = new List<NoteInfo>
        {
            new NoteInfo
            {
                NoteId = 123,
                ModelName = "Basic",
                ModificationTimestamp = 1000000003,
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Front"] = new NoteFieldInfo { Value = "Question?" },
                    // Missing Back field
                }
            },
            new NoteInfo
            {
                NoteId = 456,
                ModelName = "Basic",
                ModificationTimestamp = 1000000004,
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Front"] = new NoteFieldInfo { Value = "Question 2?" },
                    ["Back"] = new NoteFieldInfo { Value = "Answer 2!" }
                }
            }
        };
        var notesInfoResponse = new NotesInfoResponse { Result = notesInfo };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        _ankiServiceMock
            .Setup(x => x.NotesInfoAsync(It.IsAny<NotesInfoRequestDto>(), default))
            .ReturnsAsync(notesInfoResponse);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull();
        result.Cards.Should().HaveCount(1); // Only the valid card should be included

        var card = result.Cards[0] as QuestionAnswerCard;
        card.Should().NotBeNull();
        card!.Id.Should().Be("456");
    }

    [Fact]
    public async Task UpsertDeck_ShouldThrowArgumentNullException_WhenDeckIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _deckRepository.UpsertDeck(null!));
    }

    [Fact]
    public async Task UpsertDeck_ShouldCreateDeckAndAddCards()
    {
        // Arrange - Use empty IDs so cards are treated as new
        var deck = new Deck
        {
            DeckId = DeckIdExtensions.FromAnkiDeckName("TestDeck"),
            Cards = new List<Card>
            {
                new QuestionAnswerCard
                {
                    Id = "",
                    DateModified = DateTimeOffset.Now,
                    Question = "What is 2+2?",
                    Answer = "4"
                },
                new ClozeCard
                {
                    Id = "",
                    DateModified = DateTimeOffset.Now,
                    Text = "The capital of France is {{c1::Paris}}."
                }
            }
        };

        _ankiServiceMock
            .Setup(x => x.CreateDeckAsync(It.IsAny<CreateDeckRequestDto>(), default))
            .ReturnsAsync(new CreateDeckResponse());

        // Mock FindNotesAsync to return empty results (no existing cards found)
        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(new FindNotesResponse { Result = new List<long>() });

        _ankiServiceMock
            .Setup(x => x.AddNoteAsync(It.IsAny<AddNoteRequestDto>(), default))
            .ReturnsAsync(new AddNoteResponse { Result = 123 });

        // Act
        await _deckRepository.UpsertDeck(deck);

        // Assert
        _ankiServiceMock.Verify(x => x.CreateDeckAsync(It.IsAny<CreateDeckRequestDto>(), default), Times.Once);
        _ankiServiceMock.Verify(x => x.AddNoteAsync(It.IsAny<AddNoteRequestDto>(), default), Times.Exactly(2));
    }

    [Fact]
    public async Task UpsertDeck_ShouldHandleEmptyCardList()
    {
        // Arrange
        var deck = new Deck
        {
            DeckId = DeckIdExtensions.FromAnkiDeckName("EmptyDeck"),
            Cards = new List<Card>()
        };

        _ankiServiceMock
            .Setup(x => x.CreateDeckAsync(It.IsAny<CreateDeckRequestDto>(), default))
            .ReturnsAsync(new CreateDeckResponse());

        // Act
        await _deckRepository.UpsertDeck(deck);

        // Assert
        _ankiServiceMock.Verify(x => x.CreateDeckAsync(It.IsAny<CreateDeckRequestDto>(), default), Times.Once);
        _ankiServiceMock.Verify(x => x.AddNoteAsync(It.IsAny<AddNoteRequestDto>(), default), Times.Never);
    }
}
