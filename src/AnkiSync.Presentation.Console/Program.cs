using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.SpacedRepetitionNotes;
using AnkiSync.Application;
using AnkiSync.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;

namespace AnkiSync.Presentation.Cli;

/// <summary>
/// Main entry point for the AnkiSync console application
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // Set up dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        var consoleService = serviceProvider.GetRequiredService<AnkiSyncConsoleService>();
        await consoleService.ExecuteAsync(args);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register application services
        services.AddApplicationServices();
        
        // Register adapters
        services.AddSpacedRepetitionNotesAdapter();
        services.AddAnkiConnectAdapter();

        // Register file system abstraction
        services.AddSingleton<IFileSystem, FileSystem>();

        // Configure logging
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole(options =>
            {
                options.FormatterName = "AnkiSyncFormatter";
            });
            logging.AddConsoleFormatter<AnkiSyncConsoleFormatter, ConsoleFormatterOptions>();
            logging.SetMinimumLevel(LogLevel.Information);
            
            // Filter to only show logs from AnkiSync namespaces
            logging.AddFilter("AnkiSync", LogLevel.Information);
            logging.AddFilter((category, level) => category?.StartsWith("AnkiSync.") ?? false);
        });

        // Register console service
        services.AddSingleton<AnkiSyncConsoleService>();
    }
}

/// <summary>
/// Custom console formatter for AnkiSync with colors and categories
/// </summary>
public class AnkiSyncConsoleFormatter : ConsoleFormatter
{
    public AnkiSyncConsoleFormatter()
        : base("AnkiSyncFormatter")
    {
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        if (logEntry.Exception == null)
        {
            WriteLogEntry(logEntry, textWriter);
        }
        else
        {
            WriteLogEntryWithException(logEntry, textWriter);
        }
    }

    private void WriteLogEntry<TState>(in LogEntry<TState> logEntry, TextWriter textWriter)
    {
        // Get the original console color
        var originalColor = Console.ForegroundColor;

        try
        {
            // Set color based on log level
            Console.ForegroundColor = GetColorForLogLevel(logEntry.LogLevel);

            // Format: [HH:mm:ss] [LEVEL] [CATEGORY] Message
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var level = GetShortLogLevel(logEntry.LogLevel);
            var category = GetCategoryName(logEntry.Category);

            textWriter.Write($"[{timestamp}] [{level}] [{category}] ");
            textWriter.Write(logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception));
            textWriter.WriteLine();
        }
        finally
        {
            // Restore original color
            Console.ForegroundColor = originalColor;
        }
    }

    private void WriteLogEntryWithException<TState>(in LogEntry<TState> logEntry, TextWriter textWriter)
    {
        WriteLogEntry(logEntry, textWriter);

        // Write exception details
        var originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (logEntry.Exception != null)
            {
                textWriter.WriteLine(logEntry.Exception.ToString());
            }
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    private static ConsoleColor GetColorForLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => ConsoleColor.Gray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Information => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.Red,
            _ => ConsoleColor.White
        };
    }

    private static string GetShortLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "UNK"
        };
    }

    private static string GetCategoryName(string fullCategoryName)
    {
        // Extract just the class name from the full namespace + class name
        var lastDotIndex = fullCategoryName.LastIndexOf('.');
        return lastDotIndex >= 0 ? fullCategoryName.Substring(lastDotIndex + 1) : fullCategoryName;
    }
}
