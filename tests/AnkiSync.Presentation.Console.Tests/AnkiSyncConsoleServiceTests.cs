using AnkiSync.Application;
using AnkiSync.Domain;
using AnkiSync.Domain.Interfaces;
using AnkiSync.Presentation.Cli;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions;
using Xunit;

namespace AnkiSync.Presentation.Console.Tests;

public class AnkiSyncConsoleServiceTests
{
    private readonly Mock<ICardSourceRepository> _mockCardSourceRepository;
    private readonly Mock<IDeckRepository> _mockDeckRepository;
    private readonly Mock<IFileSystem> _mockFileSystem;
    private readonly Mock<ILogger<CardSynchronizationService>> _mockSyncLogger;
    private readonly Mock<ILogger<AnkiSyncConsoleService>> _mockConsoleLogger;
    private readonly CardSynchronizationService _synchronizationService;
    private readonly AnkiSyncConsoleService _service;

    public AnkiSyncConsoleServiceTests()
    {
        _mockCardSourceRepository = new Mock<ICardSourceRepository>();
        _mockDeckRepository = new Mock<IDeckRepository>();
        _mockFileSystem = new Mock<IFileSystem>();
        _mockSyncLogger = new Mock<ILogger<CardSynchronizationService>>();
        _mockConsoleLogger = new Mock<ILogger<AnkiSyncConsoleService>>();
        
        _synchronizationService = new CardSynchronizationService(
            _mockCardSourceRepository.Object,
            _mockDeckRepository.Object,
            _mockSyncLogger.Object);
        
        _service = new AnkiSyncConsoleService(_synchronizationService, _mockFileSystem.Object, _mockConsoleLogger.Object);
    }

    [Fact]
    public async Task ExecuteCommandAsync_SyncCommand_CallsSynchronizationService()
    {
        // Arrange
        var directory = "C:\\TestDirectory";
        var args = new[] { directory };

        _mockCardSourceRepository
            .Setup(x => x.GetCardsFromDirectories(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Deck>());

        // Act
        await _service.ExecuteCommandAsync("sync", args);

        // Assert
        _mockCardSourceRepository.Verify(x => x.GetCardsFromDirectories(It.Is<IEnumerable<string>>(dirs => dirs.Contains(directory)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteCommandAsync_SyncLoopCommand_ValidatesDirectoryExists()
    {
        // Arrange
        var args = new[] { "C:\\TestDirectory" };
        _mockFileSystem.Setup(x => x.Directory.Exists("C:\\TestDirectory")).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ExecuteCommandAsync("sync-loop", args));
        Assert.Contains("Directory does not exist", exception.Message);
    }

    [Fact]
    public async Task ExecuteCommandAsync_StatusCommand_ThrowsNotImplementedException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _service.ExecuteCommandAsync("status", args));
    }

    [Fact]
    public async Task ExecuteCommandAsync_DecksCommand_ThrowsNotImplementedException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _service.ExecuteCommandAsync("decks", args));
    }

    [Fact]
    public async Task ExecuteCommandAsync_TestCommand_ThrowsNotImplementedException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _service.ExecuteCommandAsync("test", args));
    }

    [Fact]
    public async Task ExecuteCommandAsync_UnknownCommand_ThrowsArgumentException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ExecuteCommandAsync("unknown", args));
        Assert.Contains("Unknown command: unknown", exception.Message);
    }

    [Fact]
    public async Task ExecuteCommandAsync_SyncCommand_MissingDirectory_ThrowsArgumentException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ExecuteCommandAsync("sync", args));
        Assert.Contains("Usage: ankisync sync <directory>", exception.Message);
    }

    [Fact]
    public async Task ExecuteCommandAsync_SyncLoopCommand_MissingDirectory_ThrowsArgumentException()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ExecuteCommandAsync("sync-loop", args));
        Assert.Contains("Usage: ankisync sync-loop <directory>", exception.Message);
    }
}