using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using AnkiSync.Domain.Extensions;
using AnkiSync.Application;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

// simple helper allowed in this test file only
static class AddNoteRequestDtoExtensions
{
    public static T ParamsAs<T>(this AddNoteRequestDto req) where T : class
    {
        return req.Params as T ?? throw new InvalidCastException();
    }
}

namespace AnkiSync.IntegrationTests
{
    /// <summary>
    /// Integration tests that exercise the card extraction and synchronization path
    /// using a fake filesystem and a mocked Anki service.  These tests do not
    /// require an actual Anki instance; they verify that the correct requests
    /// would be emitted when a card is added.
    /// </summary>
    public class SpacedRepetitionSyncTests
    {
        [Fact]
        public async Task NewCardFromObsidianFile_ShouldIncludeTagsWhenCreatingNote()
        {
            // arrange - fake filesystem with one markdown file containing a card
            var filePath = "C:\\notes\\test.md";
            var fileContent = "#tag1\n\n(more-tag) Question?::Answer";
            var mockFs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { filePath, new MockFileData(fileContent) }
            });

            // build the repository pipeline components
            var parser = new FileParser();
            var extractor = new CardExtractor(new Mock<ILogger<CardExtractor>>().Object);
            var inferencer = new DeckInferencer(mockFs);
            var repoLogger = new Mock<ILogger<SpacedRepetitionNotesRepository>>();
            var repo = new SpacedRepetitionNotesRepository(parser, extractor, inferencer, mockFs, repoLogger.Object);

            // verify that parsing/extraction itself preserves the tags
            var document = await parser.ParseContentAsync(filePath, fileContent, mockFs.File.GetLastWriteTimeUtc(filePath));
            var parsedCards = extractor.ExtractCards(document).ToList();
            parsedCards.Should().HaveCount(1);
            parsedCards[0].Tags.NestedTags.Should().Equal(new[] { "tag1", "more-tag" });

            // now run the repository to get domain decks (tags will be lost here)
            var sourceDecks = (await repo.GetCardsFromFiles(new[] { filePath })).ToList();
            sourceDecks.Should().HaveCount(1);

            // prepare a mocked Anki service that records calls
            var ankiMock = new Mock<IAnkiService>(MockBehavior.Strict);
            ankiMock.Setup(x => x.CreateDeckAsync(It.IsAny<CreateDeckRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateDeckResponse());

            AddNoteRequestDto? capturedRequest = null;
            ankiMock.Setup(x => x.AddNoteAsync(It.IsAny<AddNoteRequestDto>(), It.IsAny<CancellationToken>()))
                .Callback<AddNoteRequestDto, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new AddNoteResponse { Result = 123 });

            var deckRepo = new DeckRepository(ankiMock.Object, new Mock<ILogger<DeckRepository>>().Object);

            // act - build and execute instructions manually (we only need create deck + create card)
            var instructions = new List<SynchronizationInstruction>();
            var deckId = sourceDecks[0].DeckId;

            // inspect note created by internal conversion to catch tag issue early
            var convertMethod = typeof(DeckRepository).GetMethod("ConvertCardToAnkiNote", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (convertMethod == null)
                throw new InvalidOperationException("ConvertCardToAnkiNote method not found");
            var result = convertMethod.Invoke(deckRepo, new object[] { sourceDecks[0].Cards.First(), deckId.ToAnkiDeckName() });
            var reflectiveNote = result as AnkiNote ?? throw new InvalidOperationException("unable to invoke conversion");
            reflectiveNote.Tags.Should().Equal(new[] { "tag1", "more-tag" });

            instructions.Add(new CreateDeckInstruction(deckId));
            instructions.Add(new CreateCardInstruction(deckId, sourceDecks[0].Cards.First()));
            // note: we don't need a SyncWithAnkiInstruction because we won't call the remote service

            await deckRepo.ExecuteInstructionsAsync(instructions);

            // confirm the mock was called
            ankiMock.Verify(x => x.AddNoteAsync(It.IsAny<AddNoteRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);

            // inspect captured request object if available
            capturedRequest.Should().NotBeNull();
            var req = capturedRequest!;
            var noteArg = req.ParamsAs<AddNoteParams>().Note;
            noteArg.Tags.Should().Equal(new[] { "tag1", "more-tag" });

            // and its JSON should also include tags for sanity
            req.ToString().Should().Contain("\"tags\"");

        }
    }
}
