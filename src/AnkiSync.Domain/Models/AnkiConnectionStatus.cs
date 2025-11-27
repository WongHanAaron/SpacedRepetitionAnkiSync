namespace AnkiSync.Domain;

/// <summary>
/// Anki connection status
/// </summary>
public enum AnkiConnectionStatus
{
    /// <summary>
    /// Connection not tested
    /// </summary>
    Unknown,

    /// <summary>
    /// Successfully connected to Anki
    /// </summary>
    Connected,

    /// <summary>
    /// Failed to connect to Anki
    /// </summary>
    Disconnected,

    /// <summary>
    /// Anki is not running
    /// </summary>
    AnkiNotRunning
}