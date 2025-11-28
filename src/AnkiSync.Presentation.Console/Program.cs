// using AnkiSync.Adapter.AnkiConnect;
// using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Application;
using AnkiSync.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AnkiSync.Presentation.Cli;

/// <summary>
/// Main entry point for the AnkiSync console application
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();

        if (args.Length == 0)
        {
            ShowHelp(logger);
            return;
        }

        var command = args[0].ToLower();

        try
        {
            // Set up dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            switch (command)
            {
                case "sync":
                    await HandleSyncCommand(serviceProvider, args, logger);
                    break;
                case "status":
                    // TODO: Implement status command
                    logger.LogWarning("Status command not yet implemented.");
                    break;
                case "decks":
                    // TODO: Implement decks command
                    logger.LogWarning("Decks command not yet implemented.");
                    break;
                case "test":
                    // TODO: Implement test command
                    logger.LogWarning("Test command not yet implemented.");
                    break;
                default:
                    logger.LogWarning("Unknown command: {Command}", command);
                    ShowHelp(logger);
                    break;
            }

            await Task.CompletedTask; // Keep method async for future use
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred");
        }
    }

    private static void ShowHelp(ILogger logger)
    {
        logger.LogInformation("Usage: AnkiSync <command>");
        logger.LogInformation("Commands:");
        logger.LogInformation("  sync <directory>    Synchronize flashcards from directory to Anki");
        logger.LogInformation("  status              Check Anki connection status");
        logger.LogInformation("  decks               List available Anki decks");
        logger.LogInformation("  test                Test Anki connection");
        logger.LogInformation("Examples:");
        logger.LogInformation("  AnkiSync sync C:\\MyNotes");
    }

    // TODO: Implement command handlers with new interface
    // private static async Task HandleStatusCommand(AnkiConnectClient client)
    // {
    //     logger.LogInformation("Checking Anki connection status...");
    //
    //     var status = await client.GetSyncStatusAsync();
    //
    //     logger.LogInformation("AnkiSync Status:");
    //     logger.LogInformation("  Anki Connection: {Status}", status.AnkiConnectionStatus);
    //     logger.LogInformation("  Sync Running: {Running}", status.IsRunning);
    //
    //     if (status.AnkiConnectionStatus == AnkiConnectionStatus.Connected)
    //     {
    //         logger.LogInformation("✓ Connected to Anki");
    //     }
    //     else
    //     {
    //         logger.LogError("✗ Not connected to Anki");
    //     }
    private static void ConfigureServices(IServiceCollection services)
    {
        // Register application services
        services.AddApplicationServices();
        
        // Register adapters
        services.AddSpacedRepetitionNotesAdapter();
        services.AddAnkiConnectAdapter();
    }

    private static async Task HandleSyncCommand(IServiceProvider serviceProvider, string[] args, ILogger logger)
    {
        if (args.Length < 2)
        {
            logger.LogError("Usage: ankisync sync <directory>");
            logger.LogInformation("Example: ankisync sync C:\\MyNotes");
            return;
        }

        var directory = args[1];
        
        logger.LogInformation("Synchronizing flashcards from directory: {Directory}", directory);
        
        try
        {
            var synchronizationService = serviceProvider.GetRequiredService<CardSynchronizationService>();
            await synchronizationService.SynchronizeCardsAsync(new[] { directory });
            
            logger.LogInformation("Synchronization completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Synchronization failed");
        }
    }
}
