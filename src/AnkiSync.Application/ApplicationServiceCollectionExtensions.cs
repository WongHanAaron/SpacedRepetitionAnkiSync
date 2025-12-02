using Microsoft.Extensions.DependencyInjection;

namespace AnkiSync.Application;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CardSynchronizationService>();
        services.AddScoped<ISynchronizationInstructionExecutor, SynchronizationInstructionExecutor>();
        return services;
    }
}