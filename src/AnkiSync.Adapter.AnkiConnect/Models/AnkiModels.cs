namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Represents an Anki note (flashcard)
/// </summary>
public class AnkiNote
{
    /// <summary>
    /// Unique identifier for the note
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The note type/model name
    /// </summary>
    public string ModelName { get; set; } = "Basic";

    /// <summary>
    /// The deck name
    /// </summary>
    public string DeckName { get; set; } = string.Empty;

    /// <summary>
    /// Field values for the note (Front, Back, etc.)
    /// </summary>
    public Dictionary<string, string> Fields { get; set; } = new();

    /// <summary>
    /// Tags associated with the note
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Audio files attached to the note
    /// </summary>
    public List<AnkiAudio> Audio { get; set; } = new();

    /// <summary>
    /// Video files attached to the note
    /// </summary>
    public List<AnkiVideo> Video { get; set; } = new();

    /// <summary>
    /// Picture files attached to the note
    /// </summary>
    public List<AnkiPicture> Picture { get; set; } = new();
}

/// <summary>
/// Represents an Anki deck
/// </summary>
public class AnkiDeck
{
    /// <summary>
    /// The deck name (may include hierarchy with ::)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for the deck
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Number of cards in the deck
    /// </summary>
    public int CardCount { get; set; }

    /// <summary>
    /// Number of notes in the deck
    /// </summary>
    public int NoteCount { get; set; }

    /// <summary>
    /// Whether the deck is a dynamic/filtered deck
    /// </summary>
    public bool IsDynamic { get; set; }
}

/// <summary>
/// Represents an Anki note model/template
/// </summary>
public class AnkiModel
{
    /// <summary>
    /// The model name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The field names for this model
    /// </summary>
    public List<string> Fields { get; set; } = new();

    /// <summary>
    /// The card templates for this model
    /// </summary>
    public List<AnkiCardTemplate> Templates { get; set; } = new();
}

/// <summary>
/// Represents a card template within a model
/// </summary>
public class AnkiCardTemplate
{
    /// <summary>
    /// The template name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The front side template
    /// </summary>
    public string Front { get; set; } = string.Empty;

    /// <summary>
    /// The back side template
    /// </summary>
    public string Back { get; set; } = string.Empty;
}

/// <summary>
/// Represents an audio file attachment
/// </summary>
public class AnkiAudio
{
    /// <summary>
    /// The URL or path to the audio file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The filename for the audio
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Whether to skip hashing the audio
    /// </summary>
    public bool SkipHash { get; set; }

    /// <summary>
    /// The field name to attach the audio to
    /// </summary>
    public string Fields { get; set; } = string.Empty;
}

/// <summary>
/// Represents a video file attachment
/// </summary>
public class AnkiVideo
{
    /// <summary>
    /// The URL or path to the video file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The filename for the video
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Whether to skip hashing the video
    /// </summary>
    public bool SkipHash { get; set; }

    /// <summary>
    /// The field name to attach the video to
    /// </summary>
    public string Fields { get; set; } = string.Empty;
}

/// <summary>
/// Represents a picture file attachment
/// </summary>
public class AnkiPicture
{
    /// <summary>
    /// The URL or path to the picture file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The filename for the picture
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// Whether to skip hashing the picture
    /// </summary>
    public bool SkipHash { get; set; }

    /// <summary>
    /// The field name to attach the picture to
    /// </summary>
    public string Fields { get; set; } = string.Empty;
}