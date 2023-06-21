using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.User;
using Music.Applications.Windows.ViewModels.Popup.Media;

namespace Music.Applications.Windows.Components;

public partial class IdentityUsersComponent : UserControl
{
    public IdentityUsersComponent()
    {
        InitializeComponent();
    }

    private void ItemBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void EmailText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: IdentityUser user }) return;
        if (Clipboard.GetText() == user.Email) return;
        Clipboard.SetText(user.Email);
        NotificationEvents.DisplayNotification("Copied to clipboard", "Copied");
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: IdentityUser user } element) return;
        var mainModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new IdentityUserPopupViewModel(user);

        mainModel.PopupViewModel.ChangeState(
            element,
            navigation
        );
    }

    private void Username_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: IdentityUser user}) return;
        ApplicationService.NavigateTo(new UserViewModel(user.Id));
    }
}