using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Models;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Default;

public class SettingsViewModel : ViewModelBase
{
    private bool _isDarkTheme;
    private bool _isNotSaved;

    public SettingsViewModel(SettingsModel settingsModel)
    {
        SettingsModel = settingsModel;
        IsDarkTheme = settingsModel.IsDarkTheme;
        GatewayPath = settingsModel.GatewayPath;
        Volume = settingsModel.Volume;
        DefaultRepeatType = settingsModel.DefaultRepeatType;
        DefaultShuffleType = settingsModel.DefaultShuffleType;
    }

    public SettingsModel SettingsModel { get; }

    public bool IsNotSaved
    {
        get => _isNotSaved;
        set
        {
            _isNotSaved = value;
            OnPropertyChanged();
        }
    }

    public float Volume { get; set; }
    public string GatewayPath { get; set; }
    public RepeatType DefaultRepeatType { get; set; }
    public ShuffleType DefaultShuffleType { get; set; }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            _isDarkTheme = value;
            ThemeManager.ChangeTheme(_isDarkTheme);
            OnPropertyChanged();
            IsNotSaved = CheckIsNotSaved();
        }
    }

    public override string ModelName { get; protected set; } = "Settings";

    public void SaveSettings()
    {
        SettingsModel.IsDarkTheme = IsDarkTheme;
        SettingsManager.SaveSettings(SettingsModel);
        IsNotSaved = CheckIsNotSaved();
    }

    private bool CheckIsNotSaved()
    {
        return SettingsModel.IsDarkTheme != IsDarkTheme;
    }
}