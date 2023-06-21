using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.ViewModels;

namespace Music.Applications.Windows.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Style = (Style)FindResource(typeof(Window));
    }

    private void MainWindow_OnDeactivated(object? sender, EventArgs e)
    {
        PopupEvents.Close();
    }

    private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }

    private void MainWindow_OnLocationChanged(object? sender, EventArgs e)
    {
        PopupEvents.Close();
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        PopupEvents.Close();
    }

    private void GlobalModal_OnCloseRequested(object sender, RoutedEventArgs e)
    {
        if (sender is not Dialog { DataContext: MainViewModel model }) return;
        model.DialogViewModel.Close();
    }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                PopupEvents.Close();
                DialogEvents.Close();
                break;
            case Key.F5:
                GlobalEvents.Update();
                break;
            case Key.Space:
                PlayerEvents.RequestPlayChange();
                break;
        }
    }
}