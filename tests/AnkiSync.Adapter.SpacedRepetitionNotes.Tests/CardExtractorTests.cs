using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using FluentAssertions;
using Xunit;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Tests;

public class CardExtractorTests
{
    private readonly CardExtractor _cardExtractor = new();

    [Fact]
    public void ExtractCards_WithAwsStudyGuideContent_ShouldExtractManyCards()
    {
        // Arrange
        var awsContent = @"#cloud #aws

## Module 1 Introduction to the Cloud

What does it mean to have ""On-demand Delivery"" in a cloud context?::It means the customer can access computing resources such as storage, compute power as needed.

What are the 3 deployment models for cloud resources?::1) Cloud, 2) On-premise deployment, 3) Hybrid deployment

What is the benefit of the cloud-based deployment?::The user has the flexibility to migrate the existing resources to the cloud, design and build new applications within the cloud environment, or use a combination of both";

        var document = new Document
        {
            FilePath = "aws_study_guide.md",
            LastModified = DateTimeOffset.UtcNow,
            Tags = new Tag { NestedTags = new List<string> { "cloud", "aws" } },
            Content = awsContent
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(4); // 3 single-line cards + 1 multi-line card from tags
        cards.Should().AllBeOfType<ParsedQuestionAnswerCard>();

        // Check the first 3 single-line cards
        var firstCard = (ParsedQuestionAnswerCard)cards[0];
        firstCard.Question.Should().Be("What does it mean to have \"On-demand Delivery\" in a cloud context?");
        firstCard.Answer.Should().Be("It means the customer can access computing resources such as storage, compute power as needed.");
        firstCard.SourceFilePath.Should().Be("aws_study_guide.md");

        var secondCard = (ParsedQuestionAnswerCard)cards[1];
        secondCard.Question.Should().Be("What are the 3 deployment models for cloud resources?");
        secondCard.Answer.Should().Be("1) Cloud, 2) On-premise deployment, 3) Hybrid deployment");

        var thirdCard = (ParsedQuestionAnswerCard)cards[2];
        thirdCard.Question.Should().Be("What is the benefit of the cloud-based deployment?");
        thirdCard.Answer.Should().Be("The user has the flexibility to migrate the existing resources to the cloud, design and build new applications within the cloud environment, or use a combination of both");

        // The 4th card is the multi-line card created from the tags
        var fourthCard = (ParsedQuestionAnswerCard)cards[3];
        fourthCard.Question.Should().Be("#cloud #aws");
    }
}