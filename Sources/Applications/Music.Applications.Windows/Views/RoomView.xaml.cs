using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Media.Users;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.ViewModels;
using Music.Applications.Windows.ViewModels.Media.User;

namespace Music.Applications.Windows.Views;

public partial class RoomView : UserControl
{
    public RoomView()
    {
        InitializeComponent();
    }

    private async void ConnectButton_OnClicked(object sender, RoutedEventArgs e)
    {
       if(sender is not FrameworkElement{DataContext: RoomViewModel model}) return;
       var roomCode = RoomCodeTextBox.Text;
       if(await model.TryConnectToRoom(roomCode)) RoomCodeTextBox.Text = "";
    }

    private async void CreateButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: RoomViewModel model}) return;
        await model.CreateRoom();
        RoomCodeTextBox.Text = "";
    }

    private async void LogoutButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: RoomViewModel model}) return;
        await model.LeaveRoom();
        RoomCodeTextBox.Text = "";
    }

    private void RoomCodeText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBlock textBlock) return;
        if (Clipboard.GetText() == textBlock.Text) return;
        Clipboard.SetText(textBlock.Text);
        NotificationEvents.DisplayNotification("Copied to clipboard", "Copied");
    }

    private async void DeleteButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: User user}) return;
        var model = App.ServiceProvider.GetRequiredService<RoomViewModel>();
        await model.KickUser(user.Id);
    }

    private void UsernameTextBlock_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: User user}) return;
        ApplicationService.NavigateTo(new UserViewModel(user.Id));
    }
}