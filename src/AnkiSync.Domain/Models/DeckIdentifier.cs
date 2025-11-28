namespace AnkiSync.Domain;

/// <summary>
/// Represents a deck identifier with its hierarchical path
/// </summary>
public record DeckIdentifier
{
    /// <summary>
    /// The full name of the deck (e.g., "Parent::Child::DeckName")
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// The immediate name of the deck (e.g., "DeckName")
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The parent deck names in hierarchical order (e.g., ["Parent", "Child"])
    /// </summary>
    public List<string> Parents { get; init; } = new List<string>();

    /// <summary>
    /// Creates a DeckIdentifier from a full deck name
    /// </summary>
    /// <param name="fullName">The full deck name with hierarchy</param>
    /// <returns>A new DeckIdentifier</returns>
    public static DeckIdentifier FromFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Deck name cannot be null or empty", nameof(fullName));
        }

        var parts = fullName.Split("::", StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            throw new ArgumentException("Deck name cannot be empty", nameof(fullName));
        }

        var name = parts.Last();
        var parents = parts.Length > 1 ? parts.Take(parts.Length - 1).ToList() : new List<string>();

        return new DeckIdentifier
        {
            FullName = fullName,
            Name = name,
            Parents = parents
        };
    }

    /// <summary>
    /// Returns the full hierarchical path as a string
    /// </summary>
    /// <returns>The full deck name</returns>
    public override string ToString() => FullName;
}