
using System;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace VRA.App.Services.Audio;

public sealed class AudioPlaybackService : IDisposable
{
    private WasapiOut? _output;
    private MMDevice? _device;
    private readonly object _lock = new();

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

    public void Stop()
    {
        lock (_lock)
        {
            StopInternal();
        }
    }

    private void StopInternal()
    {
        _output?.Stop();
        _output?.Dispose();
        _output = null;

        _device?.Dispose();
        _device = null;
    }

    public void Dispose()
    {
        Stop();
    }
}
