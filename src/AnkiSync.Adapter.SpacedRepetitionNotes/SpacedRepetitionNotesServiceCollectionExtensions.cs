using AnkiSync.Adapter.SpacedRepetitionNotes.Models;
using AnkiSync.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AnkiSync.Adapter.SpacedRepetitionNotes;

/// <summary>
/// Extension methods for registering Spaced Repetition Notes adapter services
/// </summary>
public static class SpacedRepetitionNotesServiceCollectionExtensions
{
    /// <summary>
    /// Adds Spaced Repetition Notes adapter services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSpacedRepetitionNotesAdapter(this IServiceCollection services)
    {
        // Register the repository implementation
        services.AddScoped<ICardSourceRepository, SpacedRepetitionNotesRepository>();

        // Register internal services
        services.AddScoped<IFileSystem, FileSystem>();
        services.AddScoped<IFileParser, FileParser>();
        services.AddScoped<ICardExtractor, CardExtractor>();
        services.AddScoped<IDeckInferencer, DeckInferencer>();

        return services;
    }
}