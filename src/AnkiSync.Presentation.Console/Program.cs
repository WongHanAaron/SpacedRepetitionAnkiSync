using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Configuration;
using AnkiSync.Domain.Core;
using AnkiSync.Domain.Core.Interfaces;
using System;

namespace AnkiSync.Presentation.Cli;

/// <summary>
/// Main entry point for the AnkiSync console application
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("AnkiSync - Synchronize flashcards from files to Anki");
        Console.WriteLine();

        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var command = args[0].ToLower();

        try
        {
            // Create AnkiConnect client with default configuration
            var options = new AnkiConnectOptions();
            using var client = new AnkiConnectClient(options);

            switch (command)
            {
                case "status":
                    await HandleStatusCommand(client);
                    break;
                case "decks":
                    await HandleDecksCommand(client);
                    break;
                case "test":
                    await HandleTestCommand(client);
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage: AnkiSync <command>");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  status    Check Anki connection status");
        Console.WriteLine("  decks     List available Anki decks");
        Console.WriteLine("  test      Test Anki connection");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  AnkiSync status");
        Console.WriteLine("  AnkiSync decks");
        Console.WriteLine("  AnkiSync test");
    }

    private static async Task HandleStatusCommand(AnkiConnectClient client)
    {
        Console.WriteLine("Checking Anki connection status...");

        var status = await client.GetSyncStatusAsync();

        Console.WriteLine("AnkiSync Status:");
        Console.WriteLine($"  Anki Connection: {status.AnkiConnectionStatus}");
        Console.WriteLine($"  Sync Running: {status.IsRunning}");

        if (status.AnkiConnectionStatus == AnkiConnectionStatus.Connected)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Connected to Anki");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✗ Not connected to Anki");
            Console.ResetColor();
        }
    }

    private static async Task HandleDecksCommand(AnkiConnectClient client)
    {
        Console.WriteLine("Retrieving Anki decks...");

        var decks = await client.GetDecksAsync();

        Console.WriteLine("Available Anki Decks:");
        if (decks != null && decks.Any())
        {
            foreach (var deck in decks.OrderBy(d => d))
            {
                Console.WriteLine($"  • {deck}");
            }
        }
        else
        {
            Console.WriteLine("  No decks found");
        }
    }

    private static async Task HandleTestCommand(AnkiConnectClient client)
    {
        Console.WriteLine("Testing Anki connection...");

        var isConnected = await client.ValidateAnkiConnectionAsync();

        if (isConnected)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Successfully connected to Anki");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✗ Failed to connect to Anki");
            Console.ResetColor();
            Console.WriteLine("Make sure Anki is running with AnkiConnect plugin installed");
        }
    }
}
