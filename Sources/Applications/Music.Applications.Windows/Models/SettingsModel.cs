using Music.Applications.Windows.Enums;
using Music.Shared.DTOs.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Music.Applications.Windows.Models;

public class SettingsModel
{
    public string GatewayPath { get; set; } = "http://localhost:8000";
    public float Volume { get; set; } = 0.2f;
    public bool IsDarkTheme { get; set; } = true;
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    //[JsonConverter(typeof(StringEnumConverter))] public LanguageEnum Language { get; set; } = LanguageEnum.EnUs;
    [JsonConverter(typeof(StringEnumConverter))]
    public RepeatType DefaultRepeatType { get; set; } = RepeatType.None;

    [JsonConverter(typeof(StringEnumConverter))]
    public ShuffleType DefaultShuffleType { get; set; } = ShuffleType.None;
}