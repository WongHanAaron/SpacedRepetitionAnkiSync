using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Xunit;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Tests;

public class DeckInferencerTests
{
    [Fact]
    public void InferDecks_CardsWithDistinctTags_InSeparateDecks()
    {
        var fs = new FileSystem();
        var inferencer = new DeckInferencer(fs);

        var card1 = new ParsedQuestionAnswerCard
        {
            Tags = new Tag { NestedTags = new List<string> { "a" } },
            SourceFilePath = "C:\\notes\\file.md",
            Question = "q1",
            Answer = "a1"
        };

        var card2 = new ParsedQuestionAnswerCard
        {
            Tags = new Tag { NestedTags = new List<string> { "b" } },
            SourceFilePath = "C:\\notes\\file.md",
            Question = "q2",
            Answer = "a2"
        };

        var decks = inferencer.InferDecks(new[] { card1, card2 }).ToList();

        decks.Should().HaveCount(2);
        decks.Select(d => string.Join("/", d.Tag.NestedTags)).Should().BeEquivalentTo(new[] { "a", "b" });
        decks.SelectMany(d => d.Cards).Should().Contain(card1);
        decks.SelectMany(d => d.Cards).Should().Contain(card2);
    }

    [Fact]
    public void InferDecks_CardWithoutTags_UsesFilePathForDeck()
    {
        var fs = new FileSystem();
        var inferencer = new DeckInferencer(fs);

        var card = new ParsedQuestionAnswerCard
        {
            Tags = new Tag { NestedTags = new List<string>() },
            SourceFilePath = "C:\\notes\\foo.md",
            Question = "q",
            Answer = "a"
        };

        var decks = inferencer.InferDecks(new[] { card }).ToList();
        decks.Should().HaveCount(1);

        // inferer will use the directory name "notes"
        decks[0].Tag.NestedTags.Should().Equal(new[] { "notes" });
        decks[0].Cards.Should().Contain(card);
    }
}
