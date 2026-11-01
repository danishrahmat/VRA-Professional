
using System.Collections.Generic;
using System.Linq;
using NAudio.CoreAudioApi;

namespace VRA.App.Services.Audio;

public sealed class AudioDeviceDiscoveryService
{
    public IReadOnlyList<AudioDeviceInfo> GetPlaybackDevices()
    {
        using var enumerator = new MMDeviceEnumerator();

        return enumerator
            .EnumerateAudioEndPoints(
                DataFlow.Render,
                DeviceState.Active)
            .Select(device => new AudioDeviceInfo
            {
                Id = device.ID,
                Name = device.FriendlyName,
                IsActive = device.State == DeviceState.Active
            })
            .ToList();
    }
}

public sealed class AudioDeviceInfo
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public bool IsActive { get; init; }
}
