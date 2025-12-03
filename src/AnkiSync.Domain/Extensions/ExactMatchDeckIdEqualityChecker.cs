using AnkiSync.Domain.Interfaces;
using AnkiSync.Domain.Models;
using System;
using System.Linq;

namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Exact-match implementation comparing Name (ordinal) and Parents sequence equality.
/// </summary>
public class ExactMatchDeckIdEqualityChecker : IDeckIdEqualityChecker
{
    public bool AreEqual(DeckId? a, DeckId? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        if (!string.Equals(a.Name, b.Name, StringComparison.Ordinal)) return false;

        var parentsA = a.Parents ?? Enumerable.Empty<string>();
        var parentsB = b.Parents ?? Enumerable.Empty<string>();
        return parentsA.SequenceEqual(parentsB);
    }
}