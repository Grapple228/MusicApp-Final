using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Controls;
using Music.Applications.Windows.ViewModels;

namespace Music.Applications.Windows.Events;

public delegate void PopupCloseRequested();

public static class PopupEvents
{
    public static event PopupCloseRequested? PopupClose;

    public static void Close()
    {
        PopupClose?.Invoke();
    }

    public static void Click(object sender, MouseButtonEventArgs e)
    {
        ProcessPopup(e);
    }

    private static void ProcessPopup(RoutedEventArgs e)
    {
        if (e.Source is CustomPopup) return;

        if (e.OriginalSource is not FrameworkElement element)
        {
            Close();
            return;
        }

        var model = App.ServiceProvider.GetRequiredService<MainViewModel>();

        if (model.PopupViewModel.Target == null)
        {
            Close();
            return;
        }

        if (!model.PopupViewModel.Target.Equals(e.Source)
            && !model.PopupViewModel.Target.Equals(element.TemplatedParent))
            Close();
    }
}