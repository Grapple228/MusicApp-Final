using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels.Default;
using Music.Applications.Windows.ViewModels.Media.User;

namespace Music.Applications.Windows.Views;

public partial class DefaultView
{
    public DefaultView()
    {
        InitializeComponent();
    }

    private void UsernameTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ApplicationService.NavigateTo<IdentityUserViewModel>();
    }

    private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ApplicationService.OnPreviewMouseWheel(sender, e);
    }

    private void SettingsButton_OnClicked(object sender, RoutedEventArgs e)
    {
        ApplicationService.NavigateTo<SettingsViewModel>();
    }

    private void LogoutButton_OnClicked(object sender, RoutedEventArgs e)
    {
        App.GetService().SignOut();
    }

    private void RefreshButton_OnClicked(object sender, RoutedEventArgs e)
    {
        GlobalEvents.Update();
    }
}