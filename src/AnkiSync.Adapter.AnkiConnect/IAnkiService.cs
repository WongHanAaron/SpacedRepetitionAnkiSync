using AnkiSync.Adapter.AnkiConnect.Models;

namespace AnkiSync.Adapter.AnkiConnect;

/// <summary>
/// Driven port for Anki operations.
/// This interface defines all interactions with the Anki application.
/// </summary>
public interface IAnkiService
{
    /// <summary>
    /// Tests the connection to Anki
    /// </summary>
    Task<VersionResponse> TestConnectionAsync(TestConnectionRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available decks from Anki
    /// </summary>
    Task<DeckNamesResponse> GetDecksAsync(GetDecksRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a deck in Anki if it doesn't exist
    /// </summary>
    Task<CreateDeckResponse> CreateDeckAsync(CreateDeckRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a note to Anki
    /// </summary>
    Task<AddNoteResponse> AddNoteAsync(AddNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing note in Anki
    /// </summary>
    Task<UpdateNoteResponse> UpdateNoteAsync(UpdateNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds existing notes matching criteria
    /// </summary>
    Task<FindNotesResponse> FindNotesAsync(FindNotesRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes decks from Anki
    /// </summary>
    Task<DeleteDecksResponse> DeleteDecksAsync(DeleteDecksRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes notes from Anki
    /// </summary>
    Task<DeleteNotesResponse> DeleteNotesAsync(DeleteNotesRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a note can be added to Anki
    /// </summary>
    Task<CanAddNoteResponse> CanAddNoteAsync(CanAddNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a note object without saving it
    /// </summary>
    Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates fields of an existing note
    /// </summary>
    Task<UpdateNoteFieldsResponse> UpdateNoteFieldsAsync(UpdateNoteFieldsRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information about notes
    /// </summary>
    Task<NotesInfoResponse> NotesInfoAsync(NotesInfoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Syncs the Anki collection with AnkiWeb
    /// </summary>
    Task<SyncResponse> SyncAsync(SyncRequestDto request, CancellationToken cancellationToken = default);
}