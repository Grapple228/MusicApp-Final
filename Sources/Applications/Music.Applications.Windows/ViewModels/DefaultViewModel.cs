using System.Collections.ObjectModel;
using System.Diagnostics;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Models;
using Music.Applications.Windows.Services;
using Music.Applications.Windows.Services.Navigation;
using Music.Applications.Windows.ViewModels.Dialog;
using Music.Applications.Windows.ViewModels.Navigation;

namespace Music.Applications.Windows.ViewModels;

public class DefaultViewModel : ViewModelBase
{
    private bool _isUpdatable;

    public DefaultViewModel(CurrentUser user,
        ApplicationNavigationService navigation,
        DialogViewModel globalDialogViewModel,
        ControlPanelViewModel controlPanelViewModel,
        NotificationViewModel notificationViewModel)
    {
        NotificationViewModel = notificationViewModel;
        User = user;
        GlobalDialogViewModel = globalDialogViewModel;
        ControlPanel = controlPanelViewModel;
        Navigation = navigation;
        PlaylistEvents.PlaylistsLoaded += PlaylistEventsOnPlaylistsLoaded;
        PlaylistEvents.PlaylistDeleted += PlaylistEventsOnPlaylistDeleted;
        PlaylistEvents.PlaylistCreated += PlaylistEventsOnPlaylistCreated;
        PlaylistEvents.PlaylistUpdated += PlaylistEventsOnPlaylistUpdated;
        Navigation.Navigated += NavigationOnNavigated;
        GlobalEvents.SignedOut += GlobalEventsOnSignedOut;
        GlobalEvents.UpdateRequested += GlobalEventsOnUpdateRequested;
    }

    public bool IsUpdatable
    {
        get => _isUpdatable;
        set
        {
            if (value == _isUpdatable) return;
            _isUpdatable = value;
            OnPropertyChanged();
        }
    }

    public CurrentUser User { get; }
    public ObservableCollection<UserPlaylist> UserPlaylists { get; } = new();
    public ApplicationNavigationService Navigation { get; }
    public NotificationViewModel NotificationViewModel { get; }
    public DialogViewModel GlobalDialogViewModel { get; }
    public ControlPanelViewModel ControlPanel { get; }

    public override string ModelName { get; protected set; } = nameof(DefaultViewModel);

    private void PlaylistEventsOnPlaylistUpdated(UserPlaylist playlist)
    {
        var existing = UserPlaylists.FirstOrDefault(x => x.Id == playlist.Id);
        if (existing == null) return;
        ApplicationService.Invoke(() => existing.Title = playlist.Title);
    }

    private void PlaylistEventsOnPlaylistCreated(UserPlaylist playlist)
    {
        if (UserPlaylists.FirstOrDefault(x => x.Id == playlist.Id) != null) return;
        ApplicationService.Invoke(() => UserPlaylists.Add(playlist));
    }

    private void PlaylistEventsOnPlaylistDeleted(Guid playlistId)
    {
        var playlist = UserPlaylists.FirstOrDefault(x => x.Id == playlistId);
        if (playlist == null) return;
        ApplicationService.Invoke(() => UserPlaylists.Remove(playlist));
    }

    private void PlaylistEventsOnPlaylistsLoaded(IEnumerable<UserPlaylist> playlists)
    {
        ApplicationService.Invoke(() => UserPlaylists.Clear());

        foreach (var userPlaylist in playlists) ApplicationService.Invoke(() => UserPlaylists.Add(userPlaylist));
    }

    public async Task Load()
    {
        try
        {
            Navigation.NavigateTo<HomeViewModel>();
            Task.Run(PlaylistEvents.Load);
        }
        catch 
        {
            // ignored
        }
    }

    ~DefaultViewModel()
    {
        Dispose();
    }

    public override void Dispose()
    {
        Navigation.Navigated -= NavigationOnNavigated;
        PlaylistEvents.PlaylistsLoaded -= PlaylistEventsOnPlaylistsLoaded;
        PlaylistEvents.PlaylistDeleted -= PlaylistEventsOnPlaylistDeleted;
        PlaylistEvents.PlaylistCreated -= PlaylistEventsOnPlaylistCreated;
        PlaylistEvents.PlaylistUpdated -= PlaylistEventsOnPlaylistUpdated;
        GlobalEvents.SignedOut -= GlobalEventsOnSignedOut;
        GlobalEvents.UpdateRequested -= GlobalEventsOnUpdateRequested; 
        GC.SuppressFinalize(this);
    }

    private static void GlobalEventsOnUpdateRequested()
    {
        Task.Run(async () => await PlaylistEvents.Load());
    }

    private void GlobalEventsOnSignedOut(string message)
    {
        Dispose();
    }

    private void NavigationOnNavigated(ViewModelBase modelBase)
    {
        IsUpdatable = modelBase is LoadableViewModel;
    }
}