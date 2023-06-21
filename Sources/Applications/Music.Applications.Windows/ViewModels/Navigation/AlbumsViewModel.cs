using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Albums;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class AlbumsViewModel : LoadableViewModel
{
    
    public AlbumsViewModel()
    {
        Task.Run(() => LoadInfo(Guid.Empty));
        MediaEvents.AlbumInfoChanged += OnAlbumInfoChanged;

        AlbumEvents.AlbumDeleted += AlbumEventsOnAlbumDeleted;
        
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public ObservableCollection<Album> Albums { get; private set; }

    public override string ModelName { get; protected set; } = "Albums";


    private void AlbumEventsOnAlbumDeleted(Guid albumId)
    {
        var album = Albums.FirstOrDefault(x => x.Id == albumId);
        if (album == null) return;
        ApplicationService.Invoke(() => Albums.Remove(album));
    }

    public override void Dispose()
    {
        MediaEvents.AlbumInfoChanged -= OnAlbumInfoChanged;

        AlbumEvents.AlbumDeleted -= AlbumEventsOnAlbumDeleted;
        
        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;

        base.Dispose();
    }

    ~AlbumsViewModel()
    {
        Dispose();
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var albums = await App.GetService().GetUserMediaAlbums();
        var albumModels = new ObservableCollection<Album>();
        foreach (var album in albums) albumModels.Add(new Album(album));
        Albums = albumModels;
        OnPropertyChanged(nameof(Albums));
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    private void OnAlbumInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var album = Albums.FirstOrDefault(x => x.Id == id);
        if (album == null) return;

        album.IsLiked = isLiked;
        album.IsBlocked = isBlocked;
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
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
}