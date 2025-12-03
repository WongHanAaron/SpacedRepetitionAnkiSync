namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Anki-specific version of ClozeCard with Anki ID
/// </summary>
public class AnkiClozeCard : AnkiSync.Domain.ClozeCard
{
    /// <summary>
    /// The Anki-specific ID for this card
    /// </summary>
    public new long Id { get; set; }

    /// <inheritdoc />
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}