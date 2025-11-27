namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Base response from AnkiConnect API
/// </summary>
public record AnkiConnectResponse<T>
{
    public required T Result { get; init; }
    public object? Error { get; init; }
}