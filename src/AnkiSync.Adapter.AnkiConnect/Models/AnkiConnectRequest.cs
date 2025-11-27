namespace AnkiSync.Adapter.AnkiConnect.Models;

/// <summary>
/// Base request for AnkiConnect API calls
/// </summary>
public record AnkiConnectRequest(string Action, int Version = 6);