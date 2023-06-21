using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Popup.Media;

namespace Music.Applications.Windows.Views.Navigation;

public partial class AdminPanelView
{
    public AdminPanelView()
    {
        InitializeComponent();
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element) return;
        var mainModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new AdminPanelPopupViewModel();

        mainModel.PopupViewModel.ChangeState(
            element,
            navigation
        );
    }

    private void GenresNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Genres;
    }

    private void UsersNavigation_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not SelectableButton { DataContext: IDisplaying displaying }) return;
        displaying.CurrentDisplaying = CurrentDisplaying.Users;
    }
}