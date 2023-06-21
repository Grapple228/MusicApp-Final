using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.User;
using Music.Applications.Windows.ViewModels.Popup.Media;
using Music.Shared.Identity.Common.Requests;

namespace Music.Applications.Windows.Views.Media.Users;

public partial class IdentityUserView : UserControl
{
    public IdentityUserView()
    {
        InitializeComponent();
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ImageButton { DataContext: IdentityUserViewModel user } button) return;
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new UserPopupViewModel(user.User,
            new NavigationPopupOptions
            {
                RemoveType = RemoveType.Delete,
                IsOwner = user.User.IsUserOwner
            });

        model.PopupViewModel.ChangeState(
            button,
            navigation
        );
    }

    private void EmailText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: IdentityUserViewModel user }) return;
        if (Clipboard.GetText() == user.User.Email) return;
        Clipboard.SetText(user.User.Email);
        NotificationEvents.DisplayNotification("Copied to clipboard", "Copied");
    }

    private async void CustomButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: IdentityUserViewModel model}) return;
        var newPass = NewPassword.Box.Password;
        var curPass = CurPassword.Box.Password;
        if (newPass.Length < 4 || curPass.Length < 4)
        {
            NotificationEvents.DisplayNotification("Password length less than 4", "Password", NotificationType.Error);
            return;
        }

        try
        {
            var service = App.GetService();
            await service.ChangePassword(new ChangePasswordRequest(curPass, newPass));
            CurPassword.Box.Password = "";
            NewPassword.Box.Password = "";
            NotificationEvents.DisplayNotification("Passwords changed", "Password");
        }
        catch
        {
            // ignored
        }
    }
}