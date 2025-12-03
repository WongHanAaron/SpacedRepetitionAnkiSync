using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain;
using AnkiSync.Domain.Models;
using AnkiSync.Adapter.AnkiConnect.Client;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AnkiSync.Adapter.AnkiConnect;

/// <summary>
/// Implementation of IDeckRepository using AnkiConnect
/// </summary>
public class DeckRepository : IDeckRepository
{
    private readonly IAnkiService _ankiService;
    private readonly ILogger<DeckRepository> _logger;

    public DeckRepository(IAnkiService ankiService, ILogger<DeckRepository> logger)
    {
        _ankiService = ankiService ?? throw new ArgumentNullException(nameof(ankiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Deck?> GetDeck(DeckId deckId, CancellationToken cancellationToken = default)
    {
        if (deckId == null)
        {
            throw new ArgumentNullException(nameof(deckId));
        }

        _logger.LogInformation("Retrieving deck {DeckId} from Anki", deckId.ToAnkiDeckName());

        var ankiDeckName = deckId.ToAnkiDeckName();
        
        // First check if the deck exists by getting all decks and checking if our deck is among them
        var decksResponse = await _ankiService.GetDecksAsync(new GetDecksRequestDto(), cancellationToken);
        if (decksResponse.Result == null || !decksResponse.Result.Contains(ankiDeckName))
        {
            _logger.LogDebug("Deck {DeckName} does not exist", ankiDeckName);
            return null;
        }

        // Find all notes in the specified deck
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

        _logger.LogInformation("Upserting deck {DeckId} with {CardCount} cards", deck.DeckId.ToAnkiDeckName(), deck.Cards.Count);

        // Create the deck if it doesn't exist
        var ankiDeckName = deck.DeckId.ToAnkiDeckName();
        var createDeckRequest = new CreateDeckRequestDto(ankiDeckName);
        await _ankiService.CreateDeckAsync(createDeckRequest, cancellationToken);

        // Process each card - update existing ones, add new ones
        foreach (var card in deck.Cards)
        {
            long? noteId = GetAnkiCardId(card);

            if (noteId.HasValue)
            {
                // Card exists (either provided ID or found by content), update it
                var ankiNote = ConvertCardToAnkiNote(card, ankiDeckName);
                var updateNoteRequest = new UpdateNoteFieldsRequestDto(noteId.Value, ankiNote.Fields);
                await _ankiService.UpdateNoteFieldsAsync(updateNoteRequest, cancellationToken);
            }
            else
            {
                // First, try to find in the target deck
                var findQueryTarget = BuildFindQuery(card, ankiDeckName);
                var findNotesRequestTarget = new FindNotesRequestDto(findQueryTarget);
                var findNotesResponseTarget = await _ankiService.FindNotesAsync(findNotesRequestTarget, cancellationToken);

                if (findNotesResponseTarget.Result?.Count > 0)
                {
                    // Found in target deck, update
                    var existingNoteId = findNotesResponseTarget.Result[0];
                    var ankiNote = ConvertCardToAnkiNote(card, ankiDeckName);
                    var updateNoteRequest = new UpdateNoteFieldsRequestDto(existingNoteId, ankiNote.Fields);
                    await _ankiService.UpdateNoteFieldsAsync(updateNoteRequest, cancellationToken);
                }
                else
                {
                    // Not in target deck, check if exists elsewhere
                    var findQueryGlobal = BuildFindQuery(card, null);
                    var findNotesRequestGlobal = new FindNotesRequestDto(findQueryGlobal);
                    var findNotesResponseGlobal = await _ankiService.FindNotesAsync(findNotesRequestGlobal, cancellationToken);

                    if (findNotesResponseGlobal.Result?.Count > 0)
                    {
                        // Exists in different deck, delete old and add new
                        var existingNoteId = findNotesResponseGlobal.Result[0];
                        var deleteNotesRequest = new DeleteNotesRequestDto(new List<long> { existingNoteId });
                        await _ankiService.DeleteNotesAsync(deleteNotesRequest, cancellationToken);

                        var ankiNote = ConvertCardToAnkiNote(card, ankiDeckName);
                        var addNoteRequest = new AddNoteRequestDto(ankiNote);
                        await _ankiService.AddNoteAsync(addNoteRequest, cancellationToken);
                    }
                    else
                    {
                        // Doesn't exist, add new
                        var ankiNote = ConvertCardToAnkiNote(card, ankiDeckName);
                        var addNoteRequest = new AddNoteRequestDto(ankiNote);
                        await _ankiService.AddNoteAsync(addNoteRequest, cancellationToken);
                    }
                }
            }
        }
    }

    private long? GetAnkiCardId(Card card)
    {
        if (card is AnkiClozeCard ankiCloze)
        {
            return ankiCloze.Id;
        }

        if (card is AnkiQuestionAnswerCard ankiQuestionAnswer)
        {
            return ankiQuestionAnswer.Id;
        }

        return null;
    }

    private string BuildFindQuery(Card card, string? deckName)
    {
        // Escape double quotes in the content
        string Escape(string s) => s.Replace("\"", "\\\"");

        var deckFilter = deckName != null ? $"deck:\"{Escape(deckName)}\" " : string.Empty;

        return card switch
        {
            AnkiQuestionAnswerCard ankiQaCard => $"{deckFilter}\"Front:{Escape(ankiQaCard.Question)}\"",
            QuestionAnswerCard qaCard => $"{deckFilter}\"Front:{Escape(qaCard.Question)}\"",
            AnkiClozeCard ankiClozeCard => $"{deckFilter}\"Text:{Escape(ankiClozeCard.Text)}\"",
            ClozeCard clozeCard => $"{deckFilter}\"Text:{Escape(clozeCard.Text)}\"",
            _ => string.Empty
        };
    }

    private Card? ConvertNoteToCard(NoteInfo note)
    {
        // Determine card type based on model name
        if (note.ModelName.Contains("Cloze", StringComparison.OrdinalIgnoreCase))
        {
            return ConvertToAnkiClozeCard(note);
        }
        else
        {
            return ConvertToAnkiQuestionAnswerCard(note);
        }
    }

    private AnkiQuestionAnswerCard? ConvertToAnkiQuestionAnswerCard(NoteInfo note)
    {
        // Basic model typically has Front and Back fields
        if (!note.Fields.TryGetValue("Front", out var frontField) ||
            !note.Fields.TryGetValue("Back", out var backField))
        {
            return null; // Skip notes that don't have the expected fields
        }

        return new AnkiQuestionAnswerCard
        {
            Id = note.NoteId,
            DateModified = note.DateModified,
            Question = frontField.Value,
            Answer = backField.Value
        };
    }

    private AnkiClozeCard? ConvertToAnkiClozeCard(NoteInfo note)
    {
        // Cloze model typically has Text field
        if (!note.Fields.TryGetValue("Text", out var textField))
        {
            return null; // Skip notes that don't have the expected fields
        }

        var (text, answers) = ConvertClozeFormatToPlaceholders(textField.Value);

        return new AnkiClozeCard
        {
            Id = note.NoteId,
            DateModified = note.DateModified,
            Text = text,
            Answers = answers
        };
    }

    private (string text, Dictionary<string, string> answers) ConvertClozeFormatToPlaceholders(string clozeText)
    {
        var text = clozeText;
        var answers = new Dictionary<string, string>();
        var clozePattern = @"{{(?:c(\d+)|([^:]+))::([^}]+)}}";
        var matches = Regex.Matches(clozeText, clozePattern);

        foreach (Match match in matches)
        {
            var answer = match.Groups[3].Value;
            string keyword;

            if (match.Groups[1].Success)
            {
                // This is a numbered cloze like {{c1::text}}, convert to named keyword
                keyword = $"answer{match.Groups[1].Value}";
            }
            else if (match.Groups[2].Success)
            {
                // This is a named keyword like {{country::France}}
                keyword = match.Groups[2].Value;
            }
            else
            {
                // Fallback for unexpected format
                keyword = $"answer{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            answers[keyword] = answer;

            var placeholder = $"{{{keyword}}}";
            text = text.Replace(match.Value, placeholder);
        }

        return (text, answers);
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
                    ["Text"] = ConvertPlaceholdersToClozeFormat(clozeCard)
                }
            },
            _ => throw new ArgumentException($"Unsupported card type: {card.GetType()}", nameof(card))
        };
    }

    private string ConvertPlaceholdersToClozeFormat(ClozeCard clozeCard)
    {
        var text = clozeCard.Text;
        var clozeIndex = 1;

        foreach (var answer in clozeCard.Answers)
        {
            var placeholder = $"{{{answer.Key}}}";
            var clozeFormat = $"{{{{c{clozeIndex}::{answer.Value}}}}}";
            text = text.Replace(placeholder, clozeFormat);
            clozeIndex++;
        }

        return text;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DeckId>> GetAllDeckIdsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all deck IDs from Anki");

        var decksResponse = await _ankiService.GetDecksAsync(new GetDecksRequestDto(), cancellationToken);
        if (decksResponse.Result == null)
        {
            return new List<DeckId>();
        }

        var deckIds = new List<DeckId>();
        foreach (var deckName in decksResponse.Result)
        {
            try
            {
                var deckId = DeckIdExtensions.FromAnkiDeckName(deckName);
                deckIds.Add(deckId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse deck name '{DeckName}' as DeckId", deckName);
            }
        }

        return deckIds;
    }

    /// <inheritdoc />
    public async Task DeleteDeckAsync(DeckId deckId, CancellationToken cancellationToken = default)
    {
        if (deckId == null)
        {
            throw new ArgumentNullException(nameof(deckId));
        }

        _logger.LogInformation("Deleting deck {DeckId} from Anki", deckId.ToAnkiDeckName());

        var ankiDeckName = deckId.ToAnkiDeckName();
        var deleteDecksRequest = new DeleteDecksRequestDto(new List<string> { ankiDeckName }, true);
        await _ankiService.DeleteDecksAsync(deleteDecksRequest, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteObsoleteCardsAsync(DeckId deckId, IEnumerable<Card> cardsToKeep, CancellationToken cancellationToken = default)
    {
        if (deckId == null)
        {
            throw new ArgumentNullException(nameof(deckId));
        }

        _logger.LogInformation("Deleting obsolete cards from deck {DeckId}", deckId.ToAnkiDeckName());

        var ankiDeckName = deckId.ToAnkiDeckName();

        // Find all notes in the deck
        var findNotesRequest = new FindNotesRequestDto($"deck:\"{ankiDeckName}\"");
        var findNotesResponse = await _ankiService.FindNotesAsync(findNotesRequest, cancellationToken);
        var existingNoteIds = findNotesResponse.Result ?? new List<long>();

        if (!existingNoteIds.Any())
        {
            return; // No notes to delete
        }

        // Get details of existing notes
        var notesInfoRequest = new NotesInfoRequestDto(existingNoteIds);
        var notesInfoResponse = await _ankiService.NotesInfoAsync(notesInfoRequest, cancellationToken);
        var existingNotes = notesInfoResponse.Result ?? new List<NoteInfo>();

        // Find notes that should be deleted (exist in Anki but not in cardsToKeep)
        var notesToDelete = new List<long>();
        var cardsToKeepList = cardsToKeep.ToList();

        foreach (var note in existingNotes)
        {
            var shouldKeep = false;

            foreach (var cardToKeep in cardsToKeepList)
            {
                if (CardMatchesNote(cardToKeep, note))
                {
                    shouldKeep = true;
                    break;
                }
            }

            if (!shouldKeep)
            {
                notesToDelete.Add(note.NoteId);
            }
        }

        if (notesToDelete.Any())
        {
            _logger.LogDebug("Deleting {Count} obsolete notes from deck {DeckId}", notesToDelete.Count, deckId.ToAnkiDeckName());
            var deleteNotesRequest = new DeleteNotesRequestDto(notesToDelete);
            await _ankiService.DeleteNotesAsync(deleteNotesRequest, cancellationToken);
        }
    }

    private bool CardMatchesNote(Card card, NoteInfo note)
    {
        return card switch
        {
            QuestionAnswerCard qaCard =>
                note.Fields.TryGetValue("Front", out var front) && front.Value == qaCard.Question &&
                note.Fields.TryGetValue("Back", out var back) && back.Value == qaCard.Answer,
            ClozeCard clozeCard =>
                note.Fields.TryGetValue("Text", out var text) && text.Value == ConvertPlaceholdersToClozeFormat(clozeCard),
            _ => false
        };
    }

    /// <inheritdoc />
    public async Task ExecuteInstructionsAsync(IEnumerable<SynchronizationInstruction> instructions, CancellationToken cancellationToken = default)
    {
        if (instructions == null)
        {
            throw new ArgumentNullException(nameof(instructions));
        }

        var instructionList = instructions.ToList();
        _logger.LogInformation("Executing {Count} synchronization instructions", instructionList.Count);

        foreach (var instruction in instructionList)
        {
            try
            {
                await ExecuteInstructionAsync(instruction, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing instruction {InstructionType}: {UniqueKey}",
                    instruction.InstructionType, instruction.GetUniqueKey());
                throw;
            }
        }

        _logger.LogInformation("Successfully executed all synchronization instructions");
    }

    private async Task ExecuteInstructionAsync(SynchronizationInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing instruction {InstructionType}: {UniqueKey}",
            instruction.InstructionType, instruction.GetUniqueKey());

        switch (instruction)
        {
            case CreateDeckInstruction createDeck:
                await ExecuteCreateDeckAsync(createDeck, cancellationToken);
                break;
            case DeleteDeckInstruction deleteDeck:
                await ExecuteDeleteDeckAsync(deleteDeck, cancellationToken);
                break;
            case CreateCardInstruction createCard:
                await ExecuteCreateCardAsync(createCard, cancellationToken);
                break;
            case UpdateCardInstruction updateCard:
                await ExecuteUpdateCardAsync(updateCard, cancellationToken);
                break;
            case DeleteCardInstruction deleteCard:
                await ExecuteDeleteCardAsync(deleteCard, cancellationToken);
                break;
            case MoveCardInstruction moveCard:
                await ExecuteMoveCardAsync(moveCard, cancellationToken);
                break;
            case SyncWithAnkiInstruction syncWithAnki:
                await ExecuteSyncWithAnkiAsync(syncWithAnki, cancellationToken);
                break;
            default:
                throw new NotSupportedException($"Instruction type {instruction.InstructionType} is not supported");
        }
    }

    private async Task ExecuteCreateDeckAsync(CreateDeckInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating deck {DeckId}", instruction.DeckId);

        var createDeckRequest = new CreateDeckRequestDto(instruction.DeckId.ToAnkiDeckName());
        await _ankiService.CreateDeckAsync(createDeckRequest, cancellationToken);
    }

    private async Task ExecuteDeleteDeckAsync(DeleteDeckInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting deck {DeckId}", instruction.DeckId);

        var deleteDecksRequest = new DeleteDecksRequestDto(new[] { instruction.DeckId.ToAnkiDeckName() }, true);
        await _ankiService.DeleteDecksAsync(deleteDecksRequest, cancellationToken);
    }

    private async Task ExecuteCreateCardAsync(CreateCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating card in deck {DeckId}", instruction.DeckId);

        var ankiNote = ConvertCardToAnkiNote(instruction.Card, instruction.DeckId.ToAnkiDeckName());
        var addNoteRequest = new AddNoteRequestDto(ankiNote);
        var response = await _ankiService.AddNoteAsync(addNoteRequest, cancellationToken);
        
        // Store the created note ID back on the card
        instruction.Card.Id = response.Result;
    }

    private async Task ExecuteUpdateCardAsync(UpdateCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating card {CardId}", instruction.CardId);

        if (!instruction.Card.Id.HasValue)
        {
            throw new InvalidOperationException("Card must have an ID to be updated");
        }

        var fields = instruction.Card switch
        {
            QuestionAnswerCard qaCard => new Dictionary<string, string>
            {
                ["Front"] = qaCard.Question,
                ["Back"] = qaCard.Answer
            },
            ClozeCard clozeCard => new Dictionary<string, string>
            {
                ["Text"] = ConvertPlaceholdersToClozeFormat(clozeCard)
            },
            _ => throw new NotSupportedException($"Card type {instruction.Card.Type} is not supported")
        };

        var updateRequest = new UpdateNoteFieldsRequestDto(instruction.Card.Id.Value, fields);
        await _ankiService.UpdateNoteFieldsAsync(updateRequest, cancellationToken);
    }

    private async Task ExecuteDeleteCardAsync(DeleteCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting card {CardId}", instruction.CardId);

        var deleteNotesRequest = new DeleteNotesRequestDto(new[] { instruction.CardId });
        await _ankiService.DeleteNotesAsync(deleteNotesRequest, cancellationToken);
    }

    private async Task ExecuteMoveCardAsync(MoveCardInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Moving card {CardId} to deck {TargetDeckId}", instruction.CardId, instruction.TargetDeckId);

        var moveRequest = new ChangeDeckRequestDto(new[] { instruction.CardId }, instruction.TargetDeckId.ToAnkiDeckName());
        await _ankiService.ChangeDeckAsync(moveRequest, cancellationToken);
    }

    private async Task ExecuteSyncWithAnkiAsync(SyncWithAnkiInstruction instruction, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Syncing with AnkiWeb");
        await SyncWithAnkiWebAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SyncWithAnkiWebAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Syncing Anki collection with AnkiWeb");

        var syncRequest = new SyncRequestDto();
        await _ankiService.SyncAsync(syncRequest, cancellationToken);
    }
}