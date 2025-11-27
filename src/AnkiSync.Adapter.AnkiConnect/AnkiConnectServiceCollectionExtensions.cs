using AnkiSync.Application.Ports.Anki;
using AnkiSync.Adapter.AnkiConnect.Client;
using AnkiSync.Adapter.AnkiConnect.Models;
using AutoMapper;
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
        // Register HttpClient for AnkiConnect
        services.AddHttpClient<IAnkiService, AnkiService>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register AutoMapper
        services.AddAutoMapper(typeof(AnkiConnectMappingProfile));

        return services;
    }
}