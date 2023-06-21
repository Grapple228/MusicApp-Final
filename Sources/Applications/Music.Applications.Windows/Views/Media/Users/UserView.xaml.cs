using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Popup;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.User;
using Music.Applications.Windows.ViewModels.Popup.Media;

namespace Music.Applications.Windows.Views.Media.Users;

public partial class UserView : UserControl
{
    public UserView()
    {
        InitializeComponent();
    }

    private void PropertiesButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not ImageButton { DataContext: UserViewModel user } button) return;
        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        var navigation = new UserPopupViewModel(user.User,
            new NavigationPopupOptions
            {
                RemoveType = RemoveType.Delete,
                IsOwner = user.User.IsUserOwner,
                IsSecondaryOwner = user.User.IsSecondaryOwner
            });

        model.PopupViewModel.ChangeState(
            button,
            navigation
        );
    }
}