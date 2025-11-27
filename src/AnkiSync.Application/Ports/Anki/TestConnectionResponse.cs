namespace AnkiSync.Application.Ports.Anki;

/// <summary>
/// Response from testing connection to Anki
/// </summary>
public record TestConnectionResponse(bool IsConnected);