using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Models.Media.Artists;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.ViewModels.Navigation;

public class ArtistsViewModel : LoadableViewModel
{
    public ArtistsViewModel()
    {
        Task.Run(() => LoadInfo(Guid.Empty));
        MediaEvents.ArtistInfoChanged += OnArtistInfoChanged;
        PlayerEvents.PlayChanged += PlayerEventsOnPlayChanged;
    }

    public ObservableCollection<Artist> Artists { get; private set; }

    public override string ModelName { get; protected set; } = "Artists";

    public override void Dispose()
    {
        MediaEvents.ArtistInfoChanged -= OnArtistInfoChanged;
        PlayerEvents.PlayChanged -= PlayerEventsOnPlayChanged;
        base.Dispose();
    }


    protected override async Task LoadMedia(Guid mediaId)
    {
        var artists = await App.GetService().GetUserMediaArtists();
        var artistModels = new ObservableCollection<Artist>();
        foreach (var artist in artists) artistModels.Add(new Artist(artist));
        Artists = artistModels;
        OnPropertyChanged(nameof(Artists));
        var control = App.ServiceProvider.GetRequiredService<ControlPanelViewModel>();
        PlayerEventsOnPlayChanged(control.TrackId, control.ContainerId, control.ContainerType, control.IsPlaying);
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    private void OnArtistInfoChanged(Guid id, bool isLiked, bool isBlocked)
    {
        var artist = Artists.FirstOrDefault(x => x.Id == id);
        if (artist == null) return;

        artist.IsLiked = isLiked;
        artist.IsBlocked = isBlocked;
    }
    
    private void PlayerEventsOnPlayChanged(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
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