
using System;
using System.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace VRA.App.Services.Audio;

public sealed class AudioPlaybackService : IDisposable
{
    private WasapiOut? _output;
    private MMDevice? _device;
    private AudioFileReader? _audioFileReader;

    private readonly object _lock = new();

    public event EventHandler<PlaybackFinishedEventArgs>? PlaybackFinished;

    // Play 1 kHz diagnostic tone
    public void PlayTestTone(string deviceId, double volume)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException(
                "Audio device ID cannot be empty.",
                nameof(deviceId));

        lock (_lock)
        {
            StopInternal();

            var enumerator = new MMDeviceEnumerator();
            MMDevice? device = null;
            WasapiOut? output = null;

            try
            {
                device = enumerator.GetDevice(deviceId);

                var tone = new SignalGenerator(44100, 1)
                {
                    Frequency = 1000,
                    Gain = Math.Clamp(volume, 0.0, 1.0),
                    Type = SignalGeneratorType.Sin
                };

                output = new WasapiOut(
                    device,
                    AudioClientShareMode.Shared,
                    true,
                    100);

                output.Init(tone.ToWaveProvider());
                output.Play();

                _device = device;
                _output = output;

                device = null;
                output = null;
            }
            finally
            {
                output?.Dispose();
                device?.Dispose();
                enumerator.Dispose();
            }
        }
    }

    // Play WAV audio through a selected device
    public void PlayWaveFile(
        string deviceId,
        string filePath,
        double volume)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException(
                "Audio device ID cannot be empty.",
                nameof(deviceId));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                "Audio file path cannot be empty.",
                nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(
                "Audio file was not found.",
                filePath);

        lock (_lock)
        {
            StopInternal();

            var enumerator = new MMDeviceEnumerator();

            MMDevice? device = null;
            WasapiOut? output = null;
            AudioFileReader? reader = null;

            try
            {
                device = enumerator.GetDevice(deviceId);

                reader = new AudioFileReader(filePath)
                {
                    Volume = (float)Math.Clamp(volume, 0.0, 1.0)
                };

                output = new WasapiOut(
                    device,
                    AudioClientShareMode.Shared,
                    true,
                    100);

                output.Init(reader);

                output.PlaybackStopped += OnPlaybackStopped;

                output.Play();

                _device = device;
                _output = output;
                _audioFileReader = reader;

                device = null;
                output = null;
                reader = null;
            }
            finally
            {
                output?.Dispose();
                reader?.Dispose();
                device?.Dispose();
                enumerator.Dispose();
            }
        }
    }

    // Called when WAV playback reaches its end or fails
    private void OnPlaybackStopped(
        object? sender,
        StoppedEventArgs e)
    {
        PlaybackFinished?.Invoke(
            this,
            new PlaybackFinishedEventArgs(e.Exception));
    }

    public void Stop()
    {
        lock (_lock)
        {
            StopInternal();
        }
    }

    private void StopInternal()
    {
        if (_output is not null)
        {
            // Unsubscribe so manual STOP doesn't report completion
            _output.PlaybackStopped -= OnPlaybackStopped;

            _output.Stop();
            _output.Dispose();
            _output = null;
        }

        _audioFileReader?.Dispose();
        _audioFileReader = null;

        _device?.Dispose();
        _device = null;
    }

    public void Dispose()
    {
        Stop();
    }
}

public sealed class PlaybackFinishedEventArgs : EventArgs
{
    public Exception? Exception { get; }

    public PlaybackFinishedEventArgs(Exception? exception)
    {
        Exception = exception;
    }
}
