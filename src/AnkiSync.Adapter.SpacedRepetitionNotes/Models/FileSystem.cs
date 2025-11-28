using System.IO;

namespace AnkiSync.Adapter.SpacedRepetitionNotes.Models;

/// <summary>
/// Implementation of IFileSystem using System.IO
/// </summary>
public class FileSystem : IFileSystem
{
    /// <summary>
    /// Determines whether the specified file exists
    /// </summary>
    public bool FileExists(string path) => File.Exists(path);

    /// <summary>
    /// Opens a text file, reads all text, and closes the file
    /// </summary>
    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);

    /// <summary>
    /// Gets information about a file
    /// </summary>
    public IFileInfo GetFileInfo(string path) => new FileInfoWrapper(new FileInfo(path));

    /// <summary>
    /// Determines whether the given path refers to an existing directory
    /// </summary>
    public bool DirectoryExists(string path) => Directory.Exists(path);

    /// <summary>
    /// Returns the names of files in the specified directory that match the specified search pattern
    /// </summary>
    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory.GetFiles(path, searchPattern, searchOption);

    /// <summary>
    /// Returns the file name of the specified path string without the extension
    /// </summary>
    public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
}

/// <summary>
/// Wrapper for FileInfo implementing IFileInfo
/// </summary>
internal class FileInfoWrapper : IFileInfo
{
    private readonly FileInfo _fileInfo;

    public FileInfoWrapper(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
    }

    public IDirectoryInfo? Directory => _fileInfo.Directory != null ? new DirectoryInfoWrapper(_fileInfo.Directory) : null;

    public DateTimeOffset LastWriteTimeUtc => new DateTimeOffset(_fileInfo.LastWriteTimeUtc);
}

/// <summary>
/// Wrapper for DirectoryInfo implementing IDirectoryInfo
/// </summary>
internal class DirectoryInfoWrapper : IDirectoryInfo
{
    private readonly DirectoryInfo _directoryInfo;

    public DirectoryInfoWrapper(DirectoryInfo directoryInfo)
    {
        _directoryInfo = directoryInfo;
    }

    public string Name => _directoryInfo.Name;

    public IDirectoryInfo? Parent => _directoryInfo.Parent != null ? new DirectoryInfoWrapper(_directoryInfo.Parent) : null;
}