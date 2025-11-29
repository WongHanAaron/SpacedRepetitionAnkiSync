using AnkiSync.Application;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System;
using System.Threading;

namespace AnkiSync.Presentation.Cli;

/// <summary>
/// Service for handling AnkiSync console commands
/// </summary>
public class AnkiSyncConsoleService
{
    private readonly CardSynchronizationService _synchronizationService;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<AnkiSyncConsoleService> _logger;

    public AnkiSyncConsoleService(
        CardSynchronizationService synchronizationService,
        IFileSystem fileSystem,
        ILogger<AnkiSyncConsoleService> logger)
    {
        _synchronizationService = synchronizationService ?? throw new ArgumentNullException(nameof(synchronizationService));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the command based on the provided arguments
    /// </summary>
    /// <param name="args">The command line arguments</param>
    /// <returns>A task representing the command execution</returns>
    public async Task ExecuteAsync(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var command = args[0].ToLower();
            await ExecuteCommandAsync(command, args.Skip(1).ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred");
        }
    }

    /// <summary>
    /// Executes the specified command with the given arguments
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="args">The command arguments</param>
    /// <returns>A task representing the command execution</returns>
    public async Task ExecuteCommandAsync(string command, string[] args)
    {
        switch (command.ToLower())
        {
            case "sync":
                await HandleSyncCommandAsync(args);
                break;
            case "sync-loop":
                await HandleSyncLoopCommandAsync(args);
                break;
            case "status":
                await HandleStatusCommandAsync();
                break;
            case "decks":
                await HandleDecksCommandAsync();
                break;
            case "test":
                await HandleTestCommandAsync();
                break;
            default:
                throw new ArgumentException($"Unknown command: {command}", nameof(command));
        }
    }

    /// <summary>
    /// Shows the help text
    /// </summary>
    public void ShowHelp()
    {
        _logger.LogInformation("Usage: AnkiSync <command>");
        _logger.LogInformation("Commands:");
        _logger.LogInformation("  sync <directory>       Synchronize flashcards from directory to Anki");
        _logger.LogInformation("  sync-loop <directory>  Continuously sync flashcards from directory to Anki on file changes and every 5 minutes");
        _logger.LogInformation("  status                 Check Anki connection status");
        _logger.LogInformation("  decks                  List available Anki decks");
        _logger.LogInformation("  test                   Test Anki connection");
        _logger.LogInformation("Examples:");
        _logger.LogInformation("  AnkiSync sync C:\\MyNotes");
        _logger.LogInformation("  AnkiSync sync-loop C:\\MyNotes");
    }

    private async Task HandleSyncCommandAsync(string[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentException("Usage: ankisync sync <directory>");
        }

        var directory = args[0];

        _logger.LogInformation("Synchronizing flashcards from directory: {Directory}", directory);

        try
        {
            await _synchronizationService.SynchronizeCardsAsync(new[] { directory });
            _logger.LogInformation("Synchronization completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synchronization failed");
            throw;
        }
    }

    private async Task HandleSyncLoopCommandAsync(string[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentException("Usage: ankisync sync-loop <directory>");
        }

        var directory = args[0];

        if (!_fileSystem.Directory.Exists(directory))
        {
            throw new ArgumentException($"Directory does not exist: {directory}", nameof(directory));
        }

        _logger.LogInformation("Starting sync loop for directory: {Directory}", directory);
        _logger.LogInformation("Sync will occur on file changes and every 5 minutes");
        _logger.LogInformation("Press Ctrl+C to stop the sync loop");

        var syncInProgress = false;
        var syncRequested = false;

        // Perform initial sync
        await PerformSyncAsync(directory);

        // Set up file system watcher
        using var watcher = _fileSystem.FileSystemWatcher.New(directory);
        watcher.IncludeSubdirectories = true;
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        watcher.Filter = "*.md"; // Only monitor markdown files

        // Set up event handlers
        watcher.Changed += (sender, e) => OnFileChanged(ref syncRequested);
        watcher.Created += (sender, e) => OnFileChanged(ref syncRequested);
        watcher.Deleted += (sender, e) => OnFileChanged(ref syncRequested);
        watcher.Renamed += (sender, e) => OnFileChanged(ref syncRequested);

        // Start watching
        watcher.EnableRaisingEvents = true;

        // Set up cancellation token for graceful shutdown
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            _logger.LogInformation("Stopping sync loop...");
        };

        // Set up periodic sync timer (every 5 minutes)
        var periodicSyncTimer = new Timer(
            callback: _ => OnPeriodicSync(ref syncRequested),
            state: null,
            dueTime: TimeSpan.FromMinutes(5), // First sync after 5 minutes
            period: TimeSpan.FromMinutes(5)); // Then every 5 minutes

        try
        {
            // Main sync loop
            while (!cts.Token.IsCancellationRequested)
            {
                if (syncRequested && !syncInProgress)
                {
                    syncInProgress = true;
                    syncRequested = false;

                    try
                    {
                        await PerformSyncAsync(directory);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Sync failed");
                    }
                    finally
                    {
                        syncInProgress = false;
                    }
                }

                await Task.Delay(500, cts.Token); // Check for changes every 500ms
            }
        }
        catch (TaskCanceledException)
        {
            // Expected when cancellation is requested
        }
        finally
        {
            // Clean up timer
            await periodicSyncTimer.DisposeAsync();
        }

        _logger.LogInformation("Sync loop stopped");
    }

    private async Task HandleStatusCommandAsync()
    {
        // TODO: Implement status command
        _logger.LogWarning("Status command not yet implemented.");
        await Task.CompletedTask; // Keep method async for future implementation
        throw new NotImplementedException("Status command not yet implemented.");
    }

    private async Task HandleDecksCommandAsync()
    {
        // TODO: Implement decks command
        _logger.LogWarning("Decks command not yet implemented.");
        await Task.CompletedTask; // Keep method async for future implementation
        throw new NotImplementedException("Decks command not yet implemented.");
    }

    private async Task HandleTestCommandAsync()
    {
        // TODO: Implement test command
        _logger.LogWarning("Test command not yet implemented.");
        await Task.CompletedTask; // Keep method async for future implementation
        throw new NotImplementedException("Test command not yet implemented.");
    }

    private void OnFileChanged(ref bool syncRequested)
    {
        if (!syncRequested)
        {
            syncRequested = true;
            _logger.LogInformation("File change detected, scheduling sync...");
        }
    }

    private void OnPeriodicSync(ref bool syncRequested)
    {
        if (!syncRequested)
        {
            syncRequested = true;
            _logger.LogInformation("Periodic sync triggered (every 5 minutes), scheduling sync...");
        }
    }

    private async Task PerformSyncAsync(string directory)
    {
        _logger.LogInformation("Starting synchronization...");
        try
        {
            await _synchronizationService.SynchronizeCardsAsync(new[] { directory });
            _logger.LogInformation("Synchronization completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synchronization failed");
            throw;
        }
    }
}
