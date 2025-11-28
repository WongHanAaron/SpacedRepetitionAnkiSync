namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Abstraction for filesystem operations
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Determines whether the specified file exists
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Opens a text file, reads all text, and closes the file
    /// </summary>
    Task<string> ReadAllTextAsync(string path);

    /// <summary>
    /// Gets information about a file
    /// </summary>
    IFileInfo GetFileInfo(string path);

    /// <summary>
    /// Determines whether the given path refers to an existing directory
    /// </summary>
    bool DirectoryExists(string path);

    /// <summary>
    /// Returns the names of files in the specified directory that match the specified search pattern
    /// </summary>
    string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

    /// <summary>
    /// Returns the file name of the specified path string without the extension
    /// </summary>
    string GetFileNameWithoutExtension(string path);
}

/// <summary>
/// Abstraction for file information
/// </summary>
public interface IFileInfo
{
    /// <summary>
    /// Gets the directory of the file
    /// </summary>
    IDirectoryInfo? Directory { get; }

    /// <summary>
    /// Gets the time when the current file was last written to
    /// </summary>
    DateTimeOffset LastWriteTimeUtc { get; }
}

/// <summary>
/// Abstraction for directory information
/// </summary>
public interface IDirectoryInfo
{
    /// <summary>
    /// Gets the name of this directory
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the parent directory
    /// </summary>
    IDirectoryInfo? Parent { get; }
}