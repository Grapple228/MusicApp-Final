using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels;

namespace Music.Applications.Windows.Components;

public partial class NotificationComponent : UserControl
{
    public NotificationComponent()
    {
        InitializeComponent();
    }

    private void CloseRequestedHandler(object sender, MouseButtonEventArgs e)
    {
        NotificationEvents.RequestClose();
    }
}