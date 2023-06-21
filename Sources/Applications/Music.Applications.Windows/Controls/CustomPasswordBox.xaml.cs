using System.Windows;
using System.Windows.Controls;

namespace Music.Applications.Windows.Controls;

public partial class CustomPasswordBox : UserControl
{
    public CustomPasswordBox()
    {
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        BoxLabel.Visibility = Box.Password.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
    }
}