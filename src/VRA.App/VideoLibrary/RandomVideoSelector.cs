
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VRA.App.VideoLibrary;

public sealed class RandomVideoSelector
{
    private readonly Random _random = new();
    private readonly Dictionary<string, string> _lastSelectedByFolder =
        new(StringComparer.OrdinalIgnoreCase);

    public ReinforcementVideo? SelectNext(
        IReadOnlyList<ReinforcementVideo> videos)
    {
        if (videos is null || videos.Count == 0)
        {
            return null;
        }

        // Ignore files that have been removed since the folder was scanned.
        var availableVideos = videos
            .Where(video =>
                !string.IsNullOrWhiteSpace(video.FilePath) &&
                File.Exists(video.FilePath))
            .ToList();

        if (availableVideos.Count == 0)
        {
            return null;
        }

        string folderId = availableVideos[0].FolderId;

        // Avoid selecting the previous video when there is more than one.
        if (availableVideos.Count > 1 &&
            _lastSelectedByFolder.TryGetValue(folderId, out var lastPath))
        {
            var alternatives = availableVideos
                .Where(video => !string.Equals(
                    video.FilePath,
                    lastPath,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (alternatives.Count > 0)
            {
                availableVideos = alternatives;
            }
        }

        var selected = availableVideos[_random.Next(availableVideos.Count)];

        _lastSelectedByFolder[folderId] = selected.FilePath;

        return selected;
    }
}
