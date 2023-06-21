using System.IO;
using Music.Applications.Windows.Models;
using Newtonsoft.Json;

namespace Music.Applications.Windows.Core;

public static class SettingsManager
{
    private static readonly string Filename = $"{Directory.GetCurrentDirectory()}/Settings.json";
    private static readonly JsonSerializer Serializer = new();

    public static SettingsModel GetSettings()
    {
        if (!File.Exists(Filename))
        {
            var settings = new SettingsModel();
            SaveSettings(settings);
            return settings;
        }

        using var sr = new StreamReader(Filename);
        using var reader = new JsonTextReader(sr);
        var result = Serializer.Deserialize<SettingsModel>(reader);
        return result ?? throw new Exception("Problem with loading of settings");
    }

    public static void LoadSettings(SettingsModel settingsModel)
    {
        var result = GetSettings();
        settingsModel.GatewayPath = result.GatewayPath;
    }

    public static void SaveSettings(SettingsModel settingsModel)
    {
        if (File.Exists(Filename)) File.Delete(Filename);

        using var sr = new StreamWriter(Filename);
        using var writer = new JsonTextWriter(sr);
        Serializer.Serialize(writer, settingsModel);
    }
}