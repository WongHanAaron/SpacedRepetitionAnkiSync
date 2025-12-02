using AnkiSync.Domain.Models;

namespace AnkiSync.Application;

/// <summary>
/// Interface for executing synchronization instructions.
/// </summary>
public interface ISynchronizationInstructionExecutor
{
    /// <summary>
    /// Executes a collection of synchronization instructions.
    /// </summary>
    /// <param name="instructions">The instructions to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteInstructionsAsync(IEnumerable<SynchronizationInstruction> instructions, CancellationToken cancellationToken = default);
}