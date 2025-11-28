using AnkiSync.Domain;
using FluentAssertions;

namespace AnkiSync.Domain.Tests;

public class DeckIdentifierTests
{
    [Fact]
    public void FromFullName_WithSimpleName_CreatesCorrectIdentifier()
    {
        // Arrange
        var fullName = "MyDeck";

        // Act
        var identifier = DeckIdentifier.FromFullName(fullName);

        // Assert
        identifier.FullName.Should().Be("MyDeck");
        identifier.Name.Should().Be("MyDeck");
        identifier.Parents.Should().BeEmpty();
    }

    [Fact]
    public void FromFullName_WithNestedName_CreatesCorrectIdentifier()
    {
        // Arrange
        var fullName = "Parent::Child::MyDeck";

        // Act
        var identifier = DeckIdentifier.FromFullName(fullName);

        // Assert
        identifier.FullName.Should().Be("Parent::Child::MyDeck");
        identifier.Name.Should().Be("MyDeck");
        identifier.Parents.Should().Equal("Parent", "Child");
    }

    [Fact]
    public void FromFullName_WithDeeplyNestedName_CreatesCorrectIdentifier()
    {
        // Arrange
        var fullName = "Level1::Level2::Level3::MyDeck";

        // Act
        var identifier = DeckIdentifier.FromFullName(fullName);

        // Assert
        identifier.FullName.Should().Be("Level1::Level2::Level3::MyDeck");
        identifier.Name.Should().Be("MyDeck");
        identifier.Parents.Should().Equal("Level1", "Level2", "Level3");
    }

    [Fact]
    public void FromFullName_WithEmptyString_ThrowsArgumentException()
    {
        // Arrange
        var fullName = "";

        // Act & Assert
        Action act = () => DeckIdentifier.FromFullName(fullName);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void FromFullName_WithWhitespaceOnly_ThrowsArgumentException()
    {
        // Arrange
        var fullName = "   ";

        // Act & Assert
        Action act = () => DeckIdentifier.FromFullName(fullName);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void FromFullName_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? fullName = null;

        // Act & Assert
        Action act = () => DeckIdentifier.FromFullName(fullName!);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void ToString_ReturnsFullName()
    {
        // Arrange
        var identifier = DeckIdentifier.FromFullName("Parent::Child::MyDeck");

        // Act
        var result = identifier.ToString();

        // Assert
        result.Should().Be("Parent::Child::MyDeck");
    }
}