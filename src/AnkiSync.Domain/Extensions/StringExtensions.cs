using System.Globalization;

namespace AnkiSync.Domain.Extensions;

/// <summary>
/// Extension methods for string operations related to Anki
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts an Obsidian tag to an Anki deck name
    /// </summary>
    /// <param name="tag">The Obsidian tag (e.g., "algorithms/datastructures")</param>
    /// <returns>Anki deck name (e.g., "Algorithms::Datastructures")</returns>
    public static string ToAnkiDeckName(this string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return "Default";
        }

        // Remove the # prefix if present
        var cleanTag = tag.TrimStart('#');

        // Split by / and capitalize each part
        var parts = cleanTag.Split('/')
            .Select(part => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(part.ToLower()))
            .ToArray();

        // Join with :: for Anki hierarchy
        return string.Join("::", parts);
    }

    /// <summary>
    /// Checks if a string is a valid Obsidian tag
    /// </summary>
    /// <param name="tag">The potential tag</param>
    /// <returns>True if valid tag format</returns>
    public static bool IsValidObsidianTag(this string tag)
    {
        return !string.IsNullOrWhiteSpace(tag) &&
               tag.StartsWith('#') &&
               tag.Length > 1 &&
               !tag.Contains(' ') &&
               tag.All(c => char.IsLetterOrDigit(c) || c == '#' || c == '/' || c == '-' || c == '_');
    }
}