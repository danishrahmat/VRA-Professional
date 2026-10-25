
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VRA.App.VideoLibrary;

public sealed class ReinforcementVideoLibraryService
{
    private readonly string _rootPath;
    private readonly RandomVideoSelector _randomVideoSelector = new();

    public ReinforcementVideoLibraryService()
    {
        _rootPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "VRA Professional",
            "ReinforcementVideos");

        EnsureFoldersExist();
    }

    public IReadOnlyList<ReinforcementVideoFolder> GetFolders()
    {
        return Enumerable.Range(1, 3)
            .Select(index =>
            {
                string id = $"folder{index}";
                string path = Path.Combine(_rootPath, id);

                return new ReinforcementVideoFolder
                {
                    Id = id,
                    Name = $"Custom Folder {index}",
                    FolderPath = path
                };
            })
            .ToList();
    }

    public IReadOnlyList<ReinforcementVideo> GetVideos(string folderId)
    {
        var folder = GetFolders().FirstOrDefault(
            item => string.Equals(
                item.Id, folderId, StringComparison.OrdinalIgnoreCase));

        if (folder is null)
        {
            throw new ArgumentException(
                "Unknown video folder.", nameof(folderId));
        }

        return Directory.EnumerateFiles(
                folder.FolderPath, "*", SearchOption.TopDirectoryOnly)
            .Where(path => string.Equals(
                Path.GetExtension(path), ".mp4",
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

    private void EnsureFoldersExist()
    {
        foreach (var folder in GetFolders())
        {
            Directory.CreateDirectory(folder.FolderPath);
        }
    }
    public ReinforcementVideo? SelectRandomVideo(string folderId)
    {
        var videos = GetVideos(folderId);
        return _randomVideoSelector.SelectNext(videos);
    }
}
