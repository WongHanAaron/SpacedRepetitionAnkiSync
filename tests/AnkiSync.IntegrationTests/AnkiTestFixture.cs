using AnkiSync.Adapter.AnkiConnect;
using AnkiSync.Adapter.AnkiConnect.Configuration;
using Xunit;

namespace AnkiSync.IntegrationTests;

/// <summary>
/// Test collection for Anki integration tests to ensure they run serially
/// and don't interfere with each other
/// </summary>
[CollectionDefinition("Anki Integration Tests")]
public class AnkiIntegrationTestCollection : ICollectionFixture<AnkiTestFixture>
{
}

/// <summary>
/// Test fixture for Anki integration tests
/// </summary>
public class AnkiTestFixture : IDisposable
{
    public bool AnkiAvailable { get; private set; }

    public AnkiTestFixture()
    {
        // Check if Anki is available during fixture setup
        AnkiAvailable = CheckAnkiAvailability().GetAwaiter().GetResult();
    }

    private async Task<bool> CheckAnkiAvailability()
    {
        try
        {
            var options = new AnkiConnectOptions();
            using var client = new AnkiConnectClient(options);
            return await client.ValidateAnkiConnectionAsync();
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}