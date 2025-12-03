using System;
using System.Collections.Generic;
using System.Linq;

namespace AnkiSync.Domain;

/// <summary>
/// Represents a deck identifier with support for nested deck hierarchies
/// </summary>
public record DeckId
{
    /// <summary>
    /// The hierarchical path components of the deck's parents, from root to immediate parent
    /// </summary>
    public IReadOnlyList<string> Parents { get; init; } = new List<string>();

    /// <summary>
    /// The immediate name of this deck
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Creates a DeckId from path components
    /// </summary>
    public static DeckId FromPath(params string[] pathComponents)
    {
        var validComponents = pathComponents.Where(p => !string.IsNullOrEmpty(p)).ToList();
        if (validComponents.Count == 0)
        {
            return new DeckId();
        }

        return new DeckId
        {
            Name = validComponents.Last(),
            Parents = validComponents.Take(validComponents.Count - 1).ToList()
        };
    }
}