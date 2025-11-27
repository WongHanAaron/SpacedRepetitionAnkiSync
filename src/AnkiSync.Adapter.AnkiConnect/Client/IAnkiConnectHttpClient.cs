using AnkiSync.Adapter.AnkiConnect.Models;

namespace AnkiSync.Adapter.AnkiConnect.Client;

/// <summary>
/// Interface for AnkiConnect HTTP client operations
/// </summary>
public interface IAnkiConnectHttpClient : IDisposable
{
    Task<T> InvokeAsync<T>(string action, CancellationToken cancellationToken = default);
    Task<T> InvokeAsync<T>(string action, object? parameters, CancellationToken cancellationToken = default);
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
}