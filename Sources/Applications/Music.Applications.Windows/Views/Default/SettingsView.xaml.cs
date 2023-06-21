using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels.Default;

namespace Music.Applications.Windows.Views.Default;

public partial class SettingsView : UserControl
{
    private readonly SettingsViewModel _model;

    public SettingsView()
    {
        InitializeComponent();
        _model = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
    }

    private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ApplicationService.OnPreviewMouseWheel(sender, e);
    }

    private void SaveButton_OnClicked(object sender, RoutedEventArgs e)
    {
        _model.SaveSettings();
    }
}