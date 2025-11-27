namespace AnkiSync.Adapter.AnkiConnect.Configuration;

/// <summary>
/// Configuration options for AnkiConnect HTTP client
/// </summary>
public class AnkiConnectOptions
{
    /// <summary>
    /// The base URL for AnkiConnect (default: http://localhost:8765)
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:8765";

    /// <summary>
    /// Timeout for HTTP requests in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum number of retry attempts for failed requests (default: 3)
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Initial delay between retries in milliseconds (default: 1000)
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Maximum delay between retries in milliseconds (default: 10000)
    /// </summary>
    public int MaxRetryDelayMs { get; set; } = 10000;

    /// <summary>
    /// Whether to use exponential backoff for retries (default: true)
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Connection pool size for HTTP client (default: 10)
    /// </summary>
    public int MaxConnectionsPerServer { get; set; } = 10;

    /// <summary>
    /// Whether to validate SSL certificates (default: false for localhost)
    /// </summary>
    public bool ValidateCertificates { get; set; } = false;
}