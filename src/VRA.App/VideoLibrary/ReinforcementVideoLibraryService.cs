
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace VRA.App.VideoLibrary;

public sealed class ReinforcementVideoLibraryService
{
    private readonly string _rootPath;
    private readonly string _settingsFilePath;
    private readonly RandomVideoSelector _randomVideoSelector = new();

    private readonly Dictionary<string, string> _folderPaths =
        new(StringComparer.OrdinalIgnoreCase);

    public ReinforcementVideoLibraryService()
    {
        _rootPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "VRA Professional",
            "ReinforcementVideos");

        _settingsFilePath = Path.Combine(
            _rootPath,
            "video-folders.json");

        Directory.CreateDirectory(_rootPath);

        LoadFolderSettings();
        EnsureFoldersExist();
    }

    public IReadOnlyList<ReinforcementVideoFolder> GetFolders()
    {
        return Enumerable.Range(1, 3)
            .Select(index =>
            {
                string id = $"folder{index}";

                return new ReinforcementVideoFolder
                {
                    Id = id,
                    Name = $"Custom Folder {index}",
                    FolderPath = _folderPaths[id]
                };
            })
            .ToList();
    }

    public void SetFolderPath(string folderId, string folderPath)
    {
        if (!_folderPaths.ContainsKey(folderId))
        {
            throw new ArgumentException(
                "Unknown video folder.", nameof(folderId));
        }

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            throw new ArgumentException(
                "Folder path cannot be empty.", nameof(folderPath));
        }

        string fullPath = Path.GetFullPath(folderPath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException(
                $"The selected video folder does not exist: {fullPath}");
        }

        _folderPaths[folderId] = fullPath;

        SaveFolderSettings();
    }

    public IReadOnlyList<ReinforcementVideo> GetVideos(string folderId)
    {
        var folder = GetFolders().FirstOrDefault(
            item => string.Equals(
                item.Id,
                folderId,
                StringComparison.OrdinalIgnoreCase));

        if (folder is null)
        {
            throw new ArgumentException(
                "Unknown video folder.", nameof(folderId));
        }

        if (!Directory.Exists(folder.FolderPath))
        {
            return [];
        }

        return Directory.EnumerateFiles(
                folder.FolderPath,
                "*",
                SearchOption.TopDirectoryOnly)
            .Where(path => string.Equals(
                Path.GetExtension(path),
                ".mp4",
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(Path.GetFileName)
            .Select(path => new ReinforcementVideo
            {
                FilePath = path,
                FileName = Path.GetFileName(path),
                FolderId = folder.Id
            })
            .ToList();
    }

    public ReinforcementVideo? SelectRandomVideo(string folderId)
    {
        var videos = GetVideos(folderId);
        return _randomVideoSelector.SelectNext(videos);
    }

    private void LoadFolderSettings()
    {
        for (int index = 1; index <= 3; index++)
        {
            string id = $"folder{index}";

            _folderPaths[id] = Path.Combine(_rootPath, id);
        }

        if (!File.Exists(_settingsFilePath))
        {
            SaveFolderSettings();
            return;
        }

        try
        {
            string json = File.ReadAllText(_settingsFilePath);

            var savedPaths =
                JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            if (savedPaths is null)
                return;

            foreach (var entry in savedPaths)
            {
                if (_folderPaths.ContainsKey(entry.Key) &&
                    !string.IsNullOrWhiteSpace(entry.Value))
                {
                    _folderPaths[entry.Key] =
                        Path.GetFullPath(entry.Value);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Could not load video folder settings: {ex.Message}");
        }
    }

    private void SaveFolderSettings()
    {
        string json = JsonSerializer.Serialize(
            _folderPaths,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_settingsFilePath, json);
    }

    private void EnsureFoldersExist()
    {
        foreach (var folder in GetFolders())
        {
            Directory.CreateDirectory(folder.FolderPath);
        }
    }
}
