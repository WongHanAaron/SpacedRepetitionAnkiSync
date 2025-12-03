using System.Collections.Generic;
using AnkiSync.Domain.Extensions;
using AnkiSync.Domain.Models;
using FluentAssertions;
using Xunit;

namespace AnkiSync.Domain.Tests;

public class ExactMatchDeckIdEqualityCheckerTests
{
    private readonly ExactMatchDeckIdEqualityChecker _checker = new ExactMatchDeckIdEqualityChecker();

    [Fact]
    public void AreEqual_SameNameAndSameParents_ReturnsTrue()
    {
        var a = new DeckId { Parents = new List<string>{"Parent1","Parent2"}, Name = "DeckName" };
        var b = new DeckId { Parents = new List<string>{"Parent1","Parent2"}, Name = "DeckName" };

        _checker.AreEqual(a, b).Should().BeTrue();
    }

    [Fact]
    public void AreEqual_SameNameDifferentParents_ReturnsFalse()
    {
        var a = new DeckId { Parents = new List<string>{"Parent1","ParentX"}, Name = "DeckName" };
        var b = new DeckId { Parents = new List<string>{"Parent1","Parent2"}, Name = "DeckName" };

        _checker.AreEqual(a, b).Should().BeFalse();
    }

    [Fact]
    public void AreEqual_DifferentName_ReturnsFalse()
    {
        var a = new DeckId { Parents = new List<string>{"Parent1","Parent2"}, Name = "DeckNameA" };
        var b = new DeckId { Parents = new List<string>{"Parent1","Parent2"}, Name = "DeckNameB" };

        _checker.AreEqual(a, b).Should().BeFalse();
    }

    [Fact]
    public void AreEqual_NullAndNull_ReturnsTrue()
    {
        DeckId? a = null;
        DeckId? b = null;

        _checker.AreEqual(a, b).Should().BeTrue();
    }

    [Fact]
    public void AreEqual_OneNull_ReturnsFalse()
    {
        DeckId? a = new DeckId { Parents = new List<string>{"P1"}, Name = "Name" };
        DeckId? b = null;

        _checker.AreEqual(a, b).Should().BeFalse();
        _checker.AreEqual(b, a).Should().BeFalse();
    }

    [Fact]
    public void AreEqual_NullParents_TreatedAsEmptySequence()
    {
        var a = new DeckId { Name = "DeckName" };
        var b = new DeckId { Parents = new List<string>(), Name = "DeckName" };

        _checker.AreEqual(a, b).Should().BeTrue();
    }

    [Fact]
    public void AreEqual_ParentOrderMatters_ReturnsFalseWhenOrderDiffers()
    {
        var a = new DeckId { Parents = new List<string>{"A","B"}, Name = "DeckName" };
        var b = new DeckId { Parents = new List<string>{"B","A"}, Name = "DeckName" };

        _checker.AreEqual(a, b).Should().BeFalse();
    }
}
