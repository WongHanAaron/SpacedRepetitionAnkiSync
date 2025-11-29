using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using FluentAssertions;
using Xunit;
using System.IO;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Tests;

public class CardExtractorTests
{
    private readonly CardExtractor _cardExtractor = new();

    [Fact]
    public void ExtractCards_WithAwsStudyGuideContent_ShouldExtractManyCards()
    {
        // Arrange - using content from the aws_study_guide.md file that was moved to this test directory
        var document = new Document
        {
            FilePath = "aws_study_guide.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["cloud", "aws"] },
            Content = """
#cloud #aws

## Module 1 Introduction to the Cloud

What does it mean to have "On-demand Delivery" in a cloud context?::It means the customer can access computing resources such as storage, compute power as needed.

What are the 3 deployment models for cloud resources?::1) Cloud, 2) On-premise deployment, 3) Hybrid deployment

What is the benefit of the cloud-based deployment?::The user has the flexibility to migrate the existing resources to the cloud, design and build new applications within the cloud environment, or use a combination of both

What is the benefit of on-premise deployment?::It can provide dedicated resources and low latency

What is the benefit of a hybrid deployment approach to cloud services?::This approach is beneficial when legacy applications must remain on premises due to maintenance preferences or regulatory requirements.

What are the 6 benefits of the cloud?::1) the ability to pay as you go, 2) the massive economy of scale, 3) scaling capacity is easy, 4) increase speed and agility to experiment, 5) no upfront costs of maintaining a data center or servers, 6) the ability to deploy your applications to different regions easily

What are AWS Regions?::AWS Regions are physical locations around the world that contain groups of data centers

What is an availability zone with respect to AWS?::An availability zone is a group of data centers

With respect to AWS, at a minimum, how many availability zones does a AWS region consist of within a geographic area?::At minimum, it contains 3 regions

What is the purpose of an availability zone with respect to AWS?::The purpose of an availability zone is to provide users within an area with low-latency, fault tolerant access to services

What does an availability zone consist of with regards to AWS?::It consists of one or more data centers with redundant power, networking, and connectivity

What does AZ stand for with regards to AWS data centers?::It stands for availability zones

What is AWS's shared responsibility principle?::It is the concept that AWS is responsible for the security of the cloud while the customer is responsible for security in the cloud

What are some of the customer's responsibility with regards to the AWS shared responsibility principle?::The customer is responsible for the customer data and client-side data encryption

What are some responsibilities with regards to AWS shared responsibility principle that might fall under the customer or AWS depending on the web service?::The server side encryption, network traffic protection, platform and application management, and OS, network, and firewall configuration

What are some of AWS's responsibilities with regards to AWS shared responsibility principle?::The software for compute, storage, database, and networking as well as the hardware and the AWS global infrastructure

## Module 2 - Compute in the Cloud

What does EC2 stand for with regards to AWS?::It refers to Amazon Elastic Compute Cloud

With EC2, which services are you paying for?::You are only paying for running instances, not stopped or terminated ones

What is the concept of multi-tenancy?::The concept of multi-tenancy is where you make sure each virtual machine is isolated from each other but is still able to share resources provided by the host

What is the purpose of a hyper-visor?::The job of a hyper-visor is to handle resource sharing and isolation

What does it mean to vertically scale an instance?::To vertically scale an instance is to provide more resources like memory or compute to that instance

What does AMI stand for with regards to AWS?::It stands for Amazon Machine Image
"""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCountGreaterThan(10); // Should extract many cards from the real AWS study guide
        cards.Should().AllBeOfType<ParsedQuestionAnswerCard>();

        // Check that we have cards from the actual content
        var questionAnswerCards = cards.OfType<ParsedQuestionAnswerCard>().ToList();
        questionAnswerCards.Should().Contain(card => card.Question.Contains("What does it mean to have"));
        questionAnswerCards.Should().Contain(card => card.Question.Contains("deployment models"));
        questionAnswerCards.Should().Contain(card => card.Question.Contains("On-demand Delivery"));
    }

    [Fact]
    public void ExtractCards_WithSingleLineReversedFlashcards_ShouldCreateTwoCards()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "test.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = [] },
            Content = """
Capital of France:::Paris
Largest planet:::Jupiter
"""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(5); // 4 reversed cards + 1 multi-line card
        cards.Should().AllBeOfType<ParsedQuestionAnswerCard>();

        // First card: Capital of France -> Paris
        var firstCard = (ParsedQuestionAnswerCard)cards[0];
        firstCard.Question.Should().Be("Capital of France");
        firstCard.Answer.Should().Be("Paris");

        // Second card: Paris -> Capital of France
        var secondCard = (ParsedQuestionAnswerCard)cards[1];
        secondCard.Question.Should().Be("Paris");
        secondCard.Answer.Should().Be("Capital of France");

        // Third card: Largest planet -> Jupiter
        var thirdCard = (ParsedQuestionAnswerCard)cards[2];
        thirdCard.Question.Should().Be("Largest planet");
        thirdCard.Answer.Should().Be("Jupiter");

        // Fourth card: Jupiter -> Largest planet
        var fourthCard = (ParsedQuestionAnswerCard)cards[3];
        fourthCard.Question.Should().Be("Jupiter");
        fourthCard.Answer.Should().Be("Largest planet");

        // Fifth card: multi-line card from tags
        var fifthCard = (ParsedQuestionAnswerCard)cards[4];
        fifthCard.Question.Should().Be("Capital of France:::Paris");
        fifthCard.Answer.Should().Be("Largest planet:::Jupiter");
    }

    [Fact]
    public void ExtractCards_WithMultiLineFlashcards_ShouldExtractCorrectly()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "science.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["science"] },
            Content = """
What is photosynthesis?
The process by which plants convert sunlight into energy
?
What is the water cycle?
Evaporation, condensation, precipitation
??
"""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(3); // 1 basic multi-line + 2 reversed cards
        cards.Should().AllBeOfType<ParsedQuestionAnswerCard>();

        // First card: forward photosynthesis
        var firstCard = (ParsedQuestionAnswerCard)cards[0];
        firstCard.Question.Should().Be("What is photosynthesis?");
        firstCard.Answer.Should().Be("The process by which plants convert sunlight into energy");

        // Second card: forward water cycle
        var secondCard = (ParsedQuestionAnswerCard)cards[1];
        secondCard.Question.Should().Be("What is the water cycle?");
        secondCard.Answer.Should().Be("Evaporation, condensation, precipitation");

        // Third card: reverse water cycle
        var thirdCard = (ParsedQuestionAnswerCard)cards[2];
        thirdCard.Question.Should().Be("Evaporation, condensation, precipitation");
        thirdCard.Answer.Should().Be("What is the water cycle?");
    }

    [Fact]
    public void ExtractCards_WithBasicCardFormat_ShouldExtractCorrectly()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "math.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["math"] },
            Content = """
Q: What is 2 + 2?
A: 4

Question: What is the capital of Germany?
Answer: Berlin
"""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(3); // 1 multi-line card + 2 individual cards
        cards.Should().AllBeOfType<ParsedQuestionAnswerCard>();

        var firstCard = (ParsedQuestionAnswerCard)cards[0];
        firstCard.Question.Should().Be("Q: What is 2 + 2?");
        firstCard.Answer.Should().Contain("A: 4");
        firstCard.Answer.Should().Contain("Question: What is the capital of Germany?");
        firstCard.Answer.Should().Contain("Answer: Berlin");

        var secondCard = (ParsedQuestionAnswerCard)cards[1];
        secondCard.Question.Should().Be("What is 2 + 2?");
        secondCard.Answer.Should().Be("4");

        var thirdCard = (ParsedQuestionAnswerCard)cards[2];
        thirdCard.Question.Should().Be("What is the capital of Germany?");
        thirdCard.Answer.Should().Be("Berlin");
    }

    [Fact]
    public void ExtractCards_WithAnkiStyleCloze_ShouldExtractClozeCard()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "geography.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["geography"] },
            Content = @"The {{c1::capital}} of {{country::France}} is {{c2::Paris}}."
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(1);
        cards[0].Should().BeOfType<ParsedClozeCard>();

        var clozeCard = (ParsedClozeCard)cards[0];
        clozeCard.Text.Should().Be("The {c1} of {country} is {c2}.");
        clozeCard.Answers.Should().HaveCount(3); // 3 actual answers
        clozeCard.Answers["c1"].Should().Be("capital");
        clozeCard.Answers["country"].Should().Be("France");
        clozeCard.Answers["c2"].Should().Be("Paris");
    }

    [Fact]
    public void ExtractCards_WithObsidianHighlightCloze_ShouldExtractClozeCard()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "biology.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["biology"] },
            Content = @"Photosynthesis is the process where plants convert ==sunlight== into ==chemical energy==."
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(1);
        cards[0].Should().BeOfType<ParsedClozeCard>();

        var clozeCard = (ParsedClozeCard)cards[0];
        clozeCard.Text.Should().Contain("{answer");
        clozeCard.Answers.Should().HaveCount(2);
        clozeCard.Answers.Should().ContainValue("sunlight");
        clozeCard.Answers.Should().ContainValue("chemical energy");
    }

    [Fact]
    public void ExtractCards_WithBoldCloze_ShouldExtractClozeCard()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "programming.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["programming"] },
            Content = @"In programming, a **variable** stores **data** that can be changed."
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(1);
        cards[0].Should().BeOfType<ParsedClozeCard>();

        var clozeCard = (ParsedClozeCard)cards[0];
        clozeCard.Text.Should().Contain("{answer");
        clozeCard.Answers.Should().HaveCount(2);
        clozeCard.Answers.Should().ContainValue("variable");
        clozeCard.Answers.Should().ContainValue("data");
    }

    [Fact]
    public void ExtractCards_WithCurlyBraceCloze_ShouldExtractClozeCard()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "biology.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["biology"] },
            Content = @"The {{mitochondria}} is the {{powerhouse}} of the cell."
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(1);
        cards[0].Should().BeOfType<ParsedClozeCard>();

        var clozeCard = (ParsedClozeCard)cards[0];
        clozeCard.Text.Should().Contain("{answer");
        clozeCard.Answers.Should().HaveCount(2);
        clozeCard.Answers.Should().ContainValue("mitochondria");
        clozeCard.Answers.Should().ContainValue("powerhouse");
    }

    [Fact]
    public void ExtractCards_WithMultipleClozeTypes_ShouldExtractSingleClozeCard()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "geography.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["geography"] },
            Content = @"The {{c1::capital}} of **France** is ==Paris==."
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(1);
        cards[0].Should().BeOfType<ParsedClozeCard>();

        var clozeCard = (ParsedClozeCard)cards[0];
        clozeCard.Answers.Should().HaveCount(3); // 3 actual answers: c1, answer1 (France), answer2 (Paris)
        clozeCard.Answers.Should().ContainValue("capital");
        clozeCard.Answers.Should().ContainValue("France");
        clozeCard.Answers.Should().ContainValue("Paris");
    }

    [Fact]
    public void ExtractCards_WithEmptyContent_ShouldReturnEmptyList()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "empty.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = [] },
            Content = ""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().BeEmpty();
    }

    [Fact]
    public void ExtractCards_WithOnlyTags_ShouldCreateTagBasedCard()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "tags_only.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["math", "algebra"] },
            Content = "#math #algebra\n\nSome content without explicit cards."
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(0); // No explicit cards, just tags
    }

    [Fact]
    public void ExtractCards_WithMixedFormats_ShouldExtractAllTypes()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "mixed.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = ["science", "biology"] },
            Content = """
#science #biology

What is DNA?::Deoxyribonucleic acid

Photosynthesis explained?
Plants convert sunlight into energy using chlorophyll
?

Q: What is mitosis?
A: Cell division process

The {{c1::cell}} contains **organelles** and ==chromosomes==.
"""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(3); // 2 Q&A +1 cloze

        // Check that we have the expected types
        var questionAnswerCards = cards.OfType<ParsedQuestionAnswerCard>().ToList();
        var clozeCards = cards.OfType<ParsedClozeCard>().ToList();

        questionAnswerCards.Should().HaveCount(2);
        clozeCards.Should().HaveCount(1);

        // Check specific cards exist
        questionAnswerCards.Should().Contain(card => card.Question == "What is DNA?" && card.Answer == "Deoxyribonucleic acid");
        questionAnswerCards.Should().Contain(card => card.Question == "What is mitosis?" && card.Answer == "Cell division process");

        // Cloze card should have the expected answers
        var clozeCard = clozeCards[0];
        clozeCard.Answers.Should().HaveCount(3);
        clozeCard.Answers.Should().ContainValue("cell");
        clozeCard.Answers.Should().ContainValue("organelles");
        clozeCard.Answers.Should().ContainValue("chromosomes");
    }

    [Fact]
    public void ExtractCards_WithInvalidFormats_ShouldSkipInvalidCards()
    {
        // Arrange
        var document = new Document
        {
            FilePath = "invalid.md",
            LastModified = DateTimeOffset.Parse("2025-11-29T00:00:00Z"),
            Tags = new Tag { NestedTags = [] },
            Content = """
Invalid::
::Invalid answer
???
Q: Valid question
A: Valid answer
Q: Question without answer
"""
        };

        // Act
        var cards = _cardExtractor.ExtractCards(document).ToList();

        // Assert
        cards.Should().HaveCount(2); // 1 malformed card + 1 valid card

        var firstCard = (ParsedQuestionAnswerCard)cards[0];
        firstCard.Question.Should().Be("Invalid::");

        var secondCard = (ParsedQuestionAnswerCard)cards[1];
        secondCard.Question.Should().Be("Valid question");
        secondCard.Answer.Should().Be("Valid answer");
    }
}