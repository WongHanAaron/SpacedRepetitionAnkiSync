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
    public async Task<Deck> GetDeck(DeckId deckId, CancellationToken cancellationToken = default)
    {
        if (deckId == null)
        {
            throw new ArgumentNullException(nameof(deckId));
        }

        // Find all notes in the specified deck
        var ankiDeckName = deckId.ToAnkiDeckName();
        var findNotesRequest = new FindNotesRequestDto($"deck:\"{ankiDeckName}\"");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest, cancellationToken);

        if (findNotesResponse.Result == null || !findNotesResponse.Result.Any())
        {
            // Return empty deck if no notes found
            return new Deck { DeckId = deckId };
        }

        // Get detailed information about the notes
        var notesInfoRequest = new NotesInfoRequestDto(findNotesResponse.Result);
        var notesInfoResponse = await _ankiService.NotesInfoAsync(notesInfoRequest, cancellationToken);

        if (notesInfoResponse.Result == null)
        {
            return new Deck { DeckId = deckId };
        }

        // Convert notes to domain cards
        var cards = notesInfoResponse.Result
            .Select(note => ConvertNoteToCard(note))
            .Where(card => card != null)
            .Cast<Card>()
            .ToList();

        return new Deck
        {
            DeckId = deckId,
            Cards = cards
        };
    }

    /// <inheritdoc />
    public async Task UpsertDeck(Deck deck, CancellationToken cancellationToken = default)
    {
        if (deck == null)
        {
            throw new ArgumentNullException(nameof(deck));
        }

        if (deck.DeckId == null)
        {
            throw new ArgumentException("Deck DeckId cannot be null", nameof(deck));
        }

        // Create the deck if it doesn't exist
        var ankiDeckName = deck.DeckId.ToAnkiDeckName();
        var createDeckRequest = new CreateDeckRequestDto(ankiDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest, cancellationToken);

        // Process each card - update existing ones, add new ones
        foreach (var card in deck.Cards)
        {
            long? noteId = null;

            if (!string.IsNullOrEmpty(card.Id) && long.TryParse(card.Id, out var parsedId))
            {
                noteId = parsedId;
            }
            else
            {
                // Card ID is missing, try to find an existing card with the same content
                var query = BuildFindQuery(card, ankiDeckName);
                if (!string.IsNullOrEmpty(query))
                {
                    var findRequest = new FindNotesRequestDto(query);
                    var findResponse = await _ankiService.FindNotesAsync(findRequest, cancellationToken);
                    
                    if (findResponse.Result != null && findResponse.Result.Any())
                    {
                        noteId = findResponse.Result.First();
                        card.Id = noteId.Value.ToString(); // Update the card object with the found ID
                    }
                }
            }

            if (noteId.HasValue)
            {
                // Card exists (either provided ID or found by content), update it
                var ankiNote = ConvertCardToAnkiNote(card, ankiDeckName);
                var updateNoteRequest = new UpdateNoteFieldsRequestDto(noteId.Value, ankiNote.Fields);
                await _ankiService.UpdateNoteFieldsAsync(updateNoteRequest, cancellationToken);
            }
            else
            {
                // Card doesn't exist, add it
                var ankiNote = ConvertCardToAnkiNote(card, ankiDeckName);
                var addNoteRequest = new AddNoteRequestDto(ankiNote);
                var addNoteResponse = await _ankiService.AddNoteAsync(addNoteRequest, cancellationToken);

                // Update the card's Id with the newly created note ID
                if (addNoteResponse.Result.HasValue)
                {
                    card.Id = addNoteResponse.Result.Value.ToString();
                }
            }
        }
    }

    private string BuildFindQuery(Card card, string deckName)
    {
        // Escape double quotes in the content
        string Escape(string s) => s.Replace("\"", "\\\"");

        return card switch
        {
            QuestionAnswerCard qaCard => $"deck:\"{deckName}\" \"Front:{Escape(qaCard.Question)}\"",
            ClozeCard clozeCard => $"deck:\"{deckName}\" \"Text:{Escape(clozeCard.Text)}\"",
            _ => string.Empty
        };
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
            DateModified = note.DateModified,
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
            DateModified = note.DateModified,
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