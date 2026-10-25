using System.IO;
namespace VRA.App.VideoLibrary;

public sealed class ReinforcementVideo
{
    public string FilePath { get; init; } = string.Empty;

    public string FileName { get; init; } = string.Empty;

    public string FolderId { get; init; } = string.Empty;

    public string DisplayName =>
        Path.GetFileNameWithoutExtension(FileName);
}
