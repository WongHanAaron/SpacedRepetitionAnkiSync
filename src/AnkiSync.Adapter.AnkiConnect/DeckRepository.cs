using AnkiSync.Application;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain;

namespace AnkiSync.Adapter.AnkiConnect;

/// <summary>
/// Implementation of IDeckRepository using AnkiConnect
/// </summary>
public class DeckRepository : IDeckRepository
{
    private readonly IAnkiService _ankiService;

    public DeckRepository(IAnkiService ankiService)
    {
        _ankiService = ankiService ?? throw new ArgumentNullException(nameof(ankiService));
    }

    /// <inheritdoc />
    public async Task<Deck> DownloadDeckAsync(string deckName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(deckName))
        {
            throw new ArgumentException("Deck name cannot be null or empty", nameof(deckName));
        }

        // Find all notes in the specified deck
        var findNotesRequest = new FindNotesRequestDto($"deck:{deckName}");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest, cancellationToken);

        if (findNotesResponse.Result == null || !findNotesResponse.Result.Any())
        {
            // Return empty deck if no notes found
            return new Deck { Name = deckName };
        }

        // Get detailed information about the notes
        var notesInfoRequest = new NotesInfoRequestDto(findNotesResponse.Result);
        var notesInfoResponse = await _ankiService.NotesInfoAsync(notesInfoRequest, cancellationToken);

        if (notesInfoResponse.Result == null)
        {
            return new Deck { Name = deckName };
        }

        // Convert notes to domain cards
        var cards = notesInfoResponse.Result
            .Select(note => ConvertNoteToCard(note))
            .Where(card => card != null)
            .Cast<Card>()
            .ToList();

        // Get all deck names to find sub-decks
        var getDecksRequest = new GetDecksRequestDto();
        var getDecksResponse = await _ankiService.GetDecksAsync(getDecksRequest, cancellationToken);

        var subDeckNames = getDecksResponse.Result?
            .Where(name => name.StartsWith($"{deckName}::"))
            .Select(name => name.Substring($"{deckName}::".Length))
            .ToList() ?? new List<string>();

        return new Deck
        {
            Name = deckName,
            Cards = cards,
            SubDeckNames = subDeckNames
        };
    }

    /// <inheritdoc />
    public async Task<DeckIdentifier> UploadDeckAsync(Deck deck, CancellationToken cancellationToken = default)
    {
        if (deck == null)
        {
            throw new ArgumentNullException(nameof(deck));
        }

        if (string.IsNullOrWhiteSpace(deck.Name))
        {
            throw new ArgumentException("Deck name cannot be null or empty", nameof(deck));
        }

        // Create the deck if it doesn't exist
        var createDeckRequest = new CreateDeckRequestDto(deck.Name);
        await _ankiService.CreateDeckAsync(createDeckRequest, cancellationToken);

        // Add all cards to the deck
        foreach (var card in deck.Cards)
        {
            var ankiNote = ConvertCardToAnkiNote(card, deck.Name);
            var addNoteRequest = new AddNoteRequestDto(ankiNote);
            await _ankiService.AddNoteAsync(addNoteRequest, cancellationToken);
        }

        return DeckIdentifier.FromFullName(deck.Name);
    }

    private Card? ConvertNoteToCard(NoteInfo note)
    {
        // Determine card type based on model name
        if (note.ModelName.Contains("Cloze", StringComparison.OrdinalIgnoreCase))
        {
            return ConvertToClozeCard(note);
        }
        else
        {
            return ConvertToQuestionAnswerCard(note);
        }
    }

    private QuestionAnswerCard? ConvertToQuestionAnswerCard(NoteInfo note)
    {
        // Basic model typically has Front and Back fields
        if (!note.Fields.TryGetValue("Front", out var frontField) ||
            !note.Fields.TryGetValue("Back", out var backField))
        {
            return null; // Skip notes that don't have the expected fields
        }

        return new QuestionAnswerCard
        {
            Id = note.NoteId.ToString(),
            Question = frontField.Value,
            Answer = backField.Value
        };
    }

    private ClozeCard? ConvertToClozeCard(NoteInfo note)
    {
        // Cloze model typically has Text field
        if (!note.Fields.TryGetValue("Text", out var textField))
        {
            return null; // Skip notes that don't have the expected fields
        }

        return new ClozeCard
        {
            Id = note.NoteId.ToString(),
            Text = textField.Value
        };
    }

    private AnkiNote ConvertCardToAnkiNote(Card card, string deckName)
    {
        return card switch
        {
            QuestionAnswerCard qaCard => new AnkiNote
            {
                DeckName = deckName,
                ModelName = "Basic",
                Fields = new Dictionary<string, string>
                {
                    ["Front"] = qaCard.Question,
                    ["Back"] = qaCard.Answer
                }
            },
            ClozeCard clozeCard => new AnkiNote
            {
                DeckName = deckName,
                ModelName = "Cloze",
                Fields = new Dictionary<string, string>
                {
                    ["Text"] = clozeCard.Text
                }
            },
            _ => throw new ArgumentException($"Unsupported card type: {card.GetType()}", nameof(card))
        };
    }
}