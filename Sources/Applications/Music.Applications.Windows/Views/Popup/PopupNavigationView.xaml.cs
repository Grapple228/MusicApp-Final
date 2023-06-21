using System.Windows;
using Music.Applications.Windows.Models.Popup;

namespace Music.Applications.Windows.Views.Popup;

public partial class PopupNavigationView
{
    public PopupNavigationView()
    {
        InitializeComponent();
    }

    private void SelectableImageButton_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PopupNavigationModel model }) return;
        model.Execute();
    }
}