using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels.Popup;

namespace Music.Applications.Windows.Views;

public partial class ChangeUsernameView : UserControl
{
    public ChangeUsernameView()
    {
        InitializeComponent();
    }

    private async void ChangeButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if(sender is not FrameworkElement{DataContext: ChangeUsernameViewModel model}) return;
        if(model.Username.Length < 4) return;

        try
        {
            var service = App.GetService();
            await service.ChangeUsername(model.Username);
            UserEvents.ChangeUsername(model.UserId, model.Username);
            DialogEvents.Close();
        }
        catch
        {
            // ignored
        }
    }
}