using AnkiSync.Domain;
using AnkiSync.Domain.Exceptions;
using AnkiSync.Domain.Extensions;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class ClozeCardTests
{
    [Fact]
    public void ClozeCard_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var card = new ClozeCard
        {
            Id = Guid.NewGuid().ToString(),
            DateModified = DateTimeOffset.Now
        };

        // Assert
        card.Id.Should().NotBeNull();
        card.Id.Should().NotBe(Guid.Empty.ToString());
        card.DateModified.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
        card.Text.Should().BeEmpty();
        card.Type.Should().Be(CardType.Cloze);
    }

    [Fact]
    public void ClozeCard_Validate_ThrowsWhenTextEmpty()
    {
        // Arrange
        var card = new ClozeCard
        {
            Id = Guid.NewGuid().ToString(),
            DateModified = DateTimeOffset.Now,
            Text = string.Empty
        };

        // Act & Assert
        card.Invoking(c => c.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("text cannot be empty");
    }

    [Fact]
    public void ClozeCard_Validate_ThrowsWhenNoClozeDeletions()
    {
        // Arrange
        var card = new ClozeCard
        {
            Id = Guid.NewGuid().ToString(),
            DateModified = DateTimeOffset.Now,
            Text = "This is a regular text without cloze deletions"
        };

        // Act & Assert
        card.Invoking(c => c.Validate())
            .Should().Throw<ValidationException>()
            .Which.Message.ToLower().Should().Contain("must contain cloze deletions");
    }

    [Fact]
    public void ClozeCard_Validate_SucceedsWhenValidClozeDeletions()
    {
        // Arrange
        var card = new ClozeCard
        {
            Id = Guid.NewGuid().ToString(),
            DateModified = DateTimeOffset.Now,
            Text = "The capital of {{c1::France}} is {{c1::Paris}}"
        };

        // Act & Assert
        card.Invoking(c => c.Validate())
            .Should().NotThrow();
    }
}