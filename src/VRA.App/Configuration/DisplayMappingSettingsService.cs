
using System.IO;
using System.Text.Json;

namespace VRA.App.Configuration;

public class DisplayMappingSettingsService
{
    private readonly string _settingsFilePath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public DisplayMappingSettingsService()
    {
        string appDataFolder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "VRA Professional");

        Directory.CreateDirectory(appDataFolder);

        _settingsFilePath = Path.Combine(
            appDataFolder,
            "display-mappings.json");
    }

    public DisplayMappingSettings Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return new DisplayMappingSettings();
        }

        try
        {
            string json = File.ReadAllText(_settingsFilePath);

            return JsonSerializer.Deserialize<DisplayMappingSettings>(
                       json,
                       JsonOptions)
                   ?? new DisplayMappingSettings();
        }
        catch (JsonException)
        {
            // Keep the application usable if the settings file is invalid.
            return new DisplayMappingSettings();
        }
        catch (IOException)
        {
            return new DisplayMappingSettings();
        }
    }

    public void Save(DisplayMappingSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        string json = JsonSerializer.Serialize(settings, JsonOptions);

        File.WriteAllText(_settingsFilePath, json);
    }
}
