using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using AnkiSync.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace AnkiSync.Adapter.AnkiConnect;

/// <summary>
/// Extension methods for registering AnkiConnect adapter services
/// </summary>
public static class AnkiConnectServiceCollectionExtensions
{
    /// <summary>
    /// Adds AnkiConnect adapter services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="baseAddress">The base address for AnkiConnect (default: http://localhost:8765)</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAnkiConnectAdapter(this IServiceCollection services, string baseAddress = "http://localhost:8765")
    {
        // Register HttpClient and wrap it with IHttpClient
        services.AddHttpClient<HttpClientWrapper>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register IHttpClient factory
        services.AddScoped<IHttpClient>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClientWrapper>();
            return httpClient;
        });

        // Register AnkiService with IHttpClient
        services.AddScoped<IAnkiService, AnkiService>();

        // Register application services implemented by this adapter
        services.AddScoped<IDeckRepository, DeckRepository>();

        return services;
    }
}