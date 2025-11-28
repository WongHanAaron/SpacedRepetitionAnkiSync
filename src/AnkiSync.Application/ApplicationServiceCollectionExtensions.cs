namespace AnkiSync.Application;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    /// <remarks>
    /// Application services are registered in their respective adapter implementations.
    /// This method exists for consistency and future application-level services.
    /// </remarks>
    public static void AddApplicationServices()
    {
        // Services are registered in their respective adapter layers
    }
}