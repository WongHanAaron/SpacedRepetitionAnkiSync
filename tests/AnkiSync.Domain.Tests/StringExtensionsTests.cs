using AnkiSync.Domain.Extensions;

namespace AnkiSync.Domain.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("#algorithms", "Algorithms")]
    [InlineData("#data-structures", "Data-Structures")]
    [InlineData("#algorithms/datastructures", "Algorithms::Datastructures")]
    [InlineData("#math/linear-algebra", "Math::Linear-Algebra")]
    [InlineData("#computer-science/algorithms/sorting", "Computer-Science::Algorithms::Sorting")]
    [InlineData("", "Default")]
    [InlineData(null, "Default")]
    public void ToAnkiDeckName_ConvertsTagToDeckName(string tag, string expected)
    {
        // Act
        var result = tag.ToAnkiDeckName();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("#tag", true)]
    [InlineData("#algorithms/datastructures", true)]
    [InlineData("#tag-with-dashes", true)]
    [InlineData("#tag_with_underscores", true)]
    [InlineData("#123numbers", true)]
    [InlineData("", false)]
    [InlineData("#", false)]
    [InlineData("#tag with spaces", false)]
    [InlineData("#tag@symbol", false)]
    [InlineData("notatag", false)]
    public void IsValidObsidianTag_ValidatesCorrectly(string tag, bool expected)
    {
        // Act
        var result = tag.IsValidObsidianTag();

        // Assert
        Assert.Equal(expected, result);
    }
}
