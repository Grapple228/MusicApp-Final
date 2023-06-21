using System.Windows;
using System.Windows.Input;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models.Media;
using Music.Applications.Windows.Models.Media.Tracks;

namespace Music.Applications.Windows.Views.Media.Tracks;

public partial class AddTrackView
{
    public AddTrackView()
    {
        InitializeComponent();
    }

    private void TrackBorder_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: Track track }) return;
        TrackEvents.RequestAddTrack(track.Id);
        DialogEvents.Close();
    }

    private void TrackGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        PopupEvents.Click(sender, e);
    }
}