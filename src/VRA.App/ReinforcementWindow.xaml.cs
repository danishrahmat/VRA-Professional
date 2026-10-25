
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace VRA.App.Views;

public partial class ReinforcementWindow : Window
{
    public ReinforcementWindow()
    {
        InitializeComponent();
    }

    public void SetTarget(string targetName)
    {
        TargetLabel.Text =
            $"VRA REINFORCEMENT — {targetName.ToUpperInvariant()}";
    }

    public void PlayVideo(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) ||
            !File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "The selected reinforcement video was not found.",
                filePath);
        }

        ReinforcementPlayer.Stop();
        ReinforcementPlayer.Source = new Uri(
            Path.GetFullPath(filePath),
            UriKind.Absolute);

        ReinforcementPlayer.Position = TimeSpan.Zero;
        ReinforcementPlayer.Play();
    }

    public void StopPlayback()
    {
        ReinforcementPlayer.Stop();
        ReinforcementPlayer.Close();
        ReinforcementPlayer.Source = null;
    }

    private void ReinforcementPlayer_MediaEnded(
        object sender, RoutedEventArgs e)
    {
        // Replay the same video until STOP ALL is requested.
        ReinforcementPlayer.Position = TimeSpan.Zero;
        ReinforcementPlayer.Play();
    }

    private void ReinforcementPlayer_MediaFailed(
        object sender, ExceptionRoutedEventArgs e)
    {
        Debug.WriteLine(
            $"Reinforcement video playback failed: {e.ErrorException}");

        StopPlayback();

        MessageBox.Show(
            "The reinforcement video could not be played. " +
            "Check that the file is a supported MP4.",
            "Video Playback Error",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    protected override void OnClosed(EventArgs e)
    {
        StopPlayback();
        base.OnClosed(e);
    }
}
