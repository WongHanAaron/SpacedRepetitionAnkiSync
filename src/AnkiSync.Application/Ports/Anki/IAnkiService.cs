namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Driven port for Anki operations.
/// This interface defines all interactions with the Anki application.
/// </summary>
public interface IAnkiService
{
    /// <summary>
    /// Tests the connection to Anki
    /// </summary>
    Task<TestConnectionResponse> TestConnectionAsync(TestConnectionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available decks from Anki
    /// </summary>
    Task<GetDecksResponse> GetDecksAsync(GetDecksRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a deck in Anki if it doesn't exist
    /// </summary>
    Task<CreateDeckResponse> CreateDeckAsync(CreateDeckRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a note to Anki
    /// </summary>
    Task<AddNoteResponse> AddNoteAsync(AddNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing note in Anki
    /// </summary>
    Task<UpdateNoteResponse> UpdateNoteAsync(UpdateNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds existing notes matching criteria
    /// </summary>
    Task<FindNotesResponse> FindNotesAsync(FindNotesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes decks from Anki
    /// </summary>
    Task<DeleteDecksResponse> DeleteDecksAsync(DeleteDecksRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes notes from Anki
    /// </summary>
    Task<DeleteNotesResponse> DeleteNotesAsync(DeleteNotesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a note can be added to Anki
    /// </summary>
    Task<CanAddNoteResponse> CanAddNoteAsync(CanAddNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a note object without saving it
    /// </summary>
    Task<CreateNoteResponse> CreateNoteAsync(CreateNoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates fields of an existing note
    /// </summary>
    Task<UpdateNoteFieldsResponse> UpdateNoteFieldsAsync(UpdateNoteFieldsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or removes tags from notes
    /// </summary>
    Task<AddTagsResponse> AddTagsAsync(AddTagsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tags from Anki
    /// </summary>
    Task<GetTagsResponse> GetTagsAsync(GetTagsRequest request, CancellationToken cancellationToken = default);
}