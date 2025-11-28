using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Application;
using AnkiSync.Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace AnkiSync.Adapter.AnkiConnect.Tests;

public class DeckRepositoryTests
{
    private readonly Mock<IAnkiService> _ankiServiceMock;
    private readonly DeckRepository _deckRepository;

    public DeckRepositoryTests()
    {
        _ankiServiceMock = new Mock<IAnkiService>();
        _deckRepository = new DeckRepository(_ankiServiceMock.Object);
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
        var deckId = new DeckId { Name = "TestDeck" };
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
        result.SubDeckNames.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDeck_ShouldReturnDeckWithCards_WhenNotesFound()
    {
        // Arrange
        var deckId = new DeckId { Name = "TestDeck" };
        var noteIds = new List<long> { 123, 456 };
        var findNotesResponse = new FindNotesResponse { Result = noteIds };

        var notesInfo = new List<NoteInfo>
        {
            new NoteInfo
            {
                NoteId = 123,
                ModelName = "Basic",
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
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Front"] = new NoteFieldInfo { Value = "Question 2?" },
                    ["Back"] = new NoteFieldInfo { Value = "Answer 2!" }
                }
            }
        };
        var notesInfoResponse = new NotesInfoResponse { Result = notesInfo };

        var deckNames = new List<string> { "TestDeck", "TestDeck::SubDeck1", "OtherDeck" };
        var getDecksResponse = new DeckNamesResponse { Result = deckNames };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        _ankiServiceMock
            .Setup(x => x.NotesInfoAsync(It.IsAny<NotesInfoRequestDto>(), default))
            .ReturnsAsync(notesInfoResponse);

        _ankiServiceMock
            .Setup(x => x.GetDecksAsync(It.IsAny<GetDecksRequestDto>(), default))
            .ReturnsAsync(getDecksResponse);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestDeck");
        result.Cards.Should().HaveCount(2);
        result.SubDeckNames.Should().ContainSingle().Which.Should().Be("SubDeck1");

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
        var deckId = new DeckId { Name = "TestDeck" };
        var noteIds = new List<long> { 789 };
        var findNotesResponse = new FindNotesResponse { Result = noteIds };

        var notesInfo = new List<NoteInfo>
        {
            new NoteInfo
            {
                NoteId = 789,
                ModelName = "Cloze",
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Text"] = new NoteFieldInfo { Value = "This is a {{c1::cloze}} test." }
                }
            }
        };
        var notesInfoResponse = new NotesInfoResponse { Result = notesInfo };

        var deckNames = new List<string> { "TestDeck" };
        var getDecksResponse = new DeckNamesResponse { Result = deckNames };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        _ankiServiceMock
            .Setup(x => x.NotesInfoAsync(It.IsAny<NotesInfoRequestDto>(), default))
            .ReturnsAsync(notesInfoResponse);

        _ankiServiceMock
            .Setup(x => x.GetDecksAsync(It.IsAny<GetDecksRequestDto>(), default))
            .ReturnsAsync(getDecksResponse);

        // Act
        var result = await _deckRepository.GetDeck(deckId);

        // Assert
        result.Should().NotBeNull();
        result.Cards.Should().HaveCount(1);

        var clozeCard = result.Cards[0] as ClozeCard;
        clozeCard.Should().NotBeNull();
        clozeCard!.Id.Should().Be("789");
        clozeCard.Text.Should().Be("This is a {{c1::cloze}} test.");
    }

    [Fact]
    public async Task GetDeck_ShouldSkipNotesWithMissingFields()
    {
        // Arrange
        var deckId = new DeckId { Name = "TestDeck" };
        var noteIds = new List<long> { 123, 456 };
        var findNotesResponse = new FindNotesResponse { Result = noteIds };

        var notesInfo = new List<NoteInfo>
        {
            new NoteInfo
            {
                NoteId = 123,
                ModelName = "Basic",
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
                Fields = new Dictionary<string, NoteFieldInfo>
                {
                    ["Front"] = new NoteFieldInfo { Value = "Question 2?" },
                    ["Back"] = new NoteFieldInfo { Value = "Answer 2!" }
                }
            }
        };
        var notesInfoResponse = new NotesInfoResponse { Result = notesInfo };

        var deckNames = new List<string> { "TestDeck" };
        var getDecksResponse = new DeckNamesResponse { Result = deckNames };

        _ankiServiceMock
            .Setup(x => x.FindNotesAsync(It.IsAny<FindNotesRequestDto>(), default))
            .ReturnsAsync(findNotesResponse);

        _ankiServiceMock
            .Setup(x => x.NotesInfoAsync(It.IsAny<NotesInfoRequestDto>(), default))
            .ReturnsAsync(notesInfoResponse);

        _ankiServiceMock
            .Setup(x => x.GetDecksAsync(It.IsAny<GetDecksRequestDto>(), default))
            .ReturnsAsync(getDecksResponse);

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
    public async Task UpsertDeck_ShouldThrowArgumentException_WhenDeckNameIsNullOrEmpty()
    {
        // Arrange
        var deck = new Deck { Name = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _deckRepository.UpsertDeck(deck));
    }

    [Fact]
    public async Task UpsertDeck_ShouldCreateDeckAndAddCards()
    {
        // Arrange
        var deck = new Deck
        {
            Name = "TestDeck",
            Cards = new List<Card>
            {
                new QuestionAnswerCard
                {
                    Id = "1",
                    Question = "What is 2+2?",
                    Answer = "4"
                },
                new ClozeCard
                {
                    Id = "2",
                    Text = "The capital of France is {{c1::Paris}}."
                }
            }
        };

        _ankiServiceMock
            .Setup(x => x.CreateDeckAsync(It.IsAny<CreateDeckRequestDto>(), default))
            .ReturnsAsync(new CreateDeckResponse());

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
            Name = "EmptyDeck",
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