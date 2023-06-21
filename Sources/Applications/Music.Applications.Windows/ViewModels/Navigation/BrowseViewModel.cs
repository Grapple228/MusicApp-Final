using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Albums;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Applications.Windows.Models.Media.Playlists;
using Music.Applications.Windows.Models.Media.Tracks;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class BrowseViewModel : LoadableViewModel, IDisplaying
{
    private DateTime _changeTime;
    private bool _isSearched;

    private string _searchQuery = "";

    private Timer? _timer;

    public BrowseViewModel()
    {
        Task.Run(async () => await LoadMedia(Guid.Empty));
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        switch (CurrentDisplaying)
        {
            case CurrentDisplaying.Artists:
                if (CurrentArtist != null)
                {
                    CurrentArtist.IsPlaying = false;
                    CurrentArtist.IsCurrent = false;
                    CurrentArtist = null;
                }
                if(containerType != ContainerType.Artist) return;
                var artist = Artists.FirstOrDefault(x => x.Id == containerId);
                if(artist == null) return;
                CurrentArtist = artist;
                CurrentArtist.IsCurrent = true;
                CurrentArtist.IsPlaying = isPlaying;
                break;
            case CurrentDisplaying.Tracks:
                if (CurrentTrack != null)
                {
                    CurrentTrack.IsPlaying = false;
                    CurrentTrack.IsCurrent = false;
                    CurrentTrack = null;
                }
                
                var track = Tracks.FirstOrDefault(x => x.Id == trackId);
                if(track == null) return;
                CurrentTrack = track;
                CurrentTrack.IsCurrent = true;
                CurrentTrack.IsPlaying = isPlaying;
                break;
            case CurrentDisplaying.Albums:
                if (CurrentAlbum != null)
                {
                    CurrentAlbum.IsPlaying = false;
                    CurrentAlbum.IsCurrent = false;
                    CurrentAlbum = null;
                }
                if(containerType != ContainerType.Album) return;
                var album = Albums.FirstOrDefault(x => x.Id == containerId);
                if(album == null) return;
                CurrentAlbum = album;
                CurrentAlbum.IsCurrent = true;
                CurrentAlbum.IsPlaying = isPlaying;
                break;
        }
    }

    
    public override string ModelName { get; protected set; } = "Browse";

    public bool IsSearched
    {
        get => _isSearched;
        set
        {
            if (value == _isSearched) return;
            _isSearched = value;
            OnPropertyChanged();
        }
    }

    private Task SearchTask { get; set; }
    private CancellationTokenSource? TokenSource { get; set; }
    private CancellationToken Token { get; set; }

    public ObservableCollection<Album> Albums { get; } = new();
    public ObservableCollection<Track> Tracks { get; } = new();
    public ObservableCollection<Artist> Artists { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            var val = value.Trim();
            if (val == _searchQuery) return;
            _searchQuery = val;
            _changeTime = DateTime.Now;
            if (val == "")
            {
                CancelSearch();
                ApplicationService.Invoke(() => Albums.Clear());
                IsSearched = false;
                return;
            }

            OnPropertyChanged();
            Task.Run(Reload);
        }
    }

    public CurrentDisplaying CurrentDisplaying
    {
        get => DisplayingMedia.BrowseViewModel;
        set
        {
            if (DisplayingMedia.BrowseViewModel.Equals(value)) return;
            DisplayingMedia.BrowseViewModel = value;
            OnPropertyChanged();
            Task.Run(async () => await Reload());
        }
    }


    private void StartSearch()
    {
        CancelSearch();
        _timer ??= new Timer(CheckSearchTime, 0, TimeSpan.Zero, TimeSpan.FromMilliseconds(20));
    }

    private void CancelSearch()
    {
        _timer?.Dispose();
        _timer = null;
        TokenSource?.Cancel();
    }

    private void CheckSearchTime(object? state)
    {
        if ((DateTime.Now - _changeTime).TotalSeconds < 1) return;
        TrySearch();
    }

    private void TrySearch()
    {
        CancelSearch();

        TokenSource = new CancellationTokenSource();
        Token = TokenSource.Token;
        SearchTask = Task.Run(async () =>
        {
            var service = App.GetService();
            try
            {
                var albums = await service.GetAlbums(searchQuery: SearchQuery);
                ApplicationService.Invoke(() => Albums.Clear());
                foreach (var albumDto in albums) ApplicationService.Invoke(() => Albums.Add(new Album(albumDto)));
            }
            catch
            {
                // ignored
            }
            try
            {
                var artists = await service.GetArtists(searchQuery: SearchQuery);
                ApplicationService.Invoke(() => Artists.Clear());
                foreach (var artistDto in artists) ApplicationService.Invoke(() => Artists.Add(new Artist(artistDto)));
            }
            catch
            {
                // ignored
            }

            try
            {
                var tracks = await service.GetTracks(searchQuery: SearchQuery);
                ApplicationService.Invoke(() => Tracks.Clear());
                foreach (var trackDto in tracks) ApplicationService.Invoke(() => Tracks.Add(new Track(trackDto)));
            }
            catch
            {
                // ignored
            }
            
            IsSearched = true;
            
            var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
            PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
        }, Token);
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        if (SearchQuery == "") return;
        StartSearch();
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }
    
    private IAlbum? _currentAlbum;
    
    public IAlbum? CurrentAlbum
    {
        get => _currentAlbum;
        set
        {
            if (Equals(value, _currentAlbum)) return;
            _currentAlbum = value;
            OnPropertyChanged();
        }
    }
    
    private ITrack? _currentTrack;
    
    public ITrack? CurrentTrack
    {
        get => _currentTrack;
        set
        {
            if (Equals(value, _currentTrack)) return;
            _currentTrack = value;
            OnPropertyChanged();
        }
    }
    
    private IArtist? _currentArtist;

    public IArtist? CurrentArtist
    {
        get => _currentArtist;
        set
        {
            if (Equals(value, _currentArtist)) return;
            _currentArtist = value;
            OnPropertyChanged();
        }
    }
}