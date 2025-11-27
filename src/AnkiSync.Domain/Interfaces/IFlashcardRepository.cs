namespace AnkiSync.Domain.Interfaces;

/// <summary>
/// Port for flashcard repository operations
/// </summary>
public interface IFlashcardRepository
{
    /// <summary>
    /// Gets all flashcards from the repository
    /// </summary>
    /// <returns>All flashcards</returns>
    Task<IEnumerable<Flashcard>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets flashcards by source file
    /// </summary>
    /// <param name="filePath">The source file path</param>
    /// <returns>Flashcards from the specified file</returns>
    Task<IEnumerable<Flashcard>> GetByFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a flashcard
    /// </summary>
    /// <param name="flashcard">The flashcard to save</param>
    Task SaveAsync(Flashcard flashcard, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a flashcard by ID
    /// </summary>
    /// <param name="id">The flashcard ID</param>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}