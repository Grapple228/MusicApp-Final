using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;

namespace Music.Applications.Windows.ViewModels.Media.Genre;

public class EditTrackGenreViewModel : LoadableViewModel
{
    private readonly Guid _trackId;
    private Models.Media.Tracks.Track _track = null!;

    public EditTrackGenreViewModel(Guid trackId)
    {
        _trackId = trackId;
        Task.Run(async () => await LoadInfo(trackId));
        GenreEvents.GenreAddRequest += GenreEventsOnGenreAddRequest;
        GenreEvents.GenreRemoveRequest += GenreEventsOnGenreRemoveRequest;
    }

    public override string ModelName { get; protected set; } = nameof(EditTrackGenreViewModel);
    public ObservableCollection<GenreDto> Genres { get; } = new();
    public ObservableCollection<GenreDto> NotSelectedGenres { get; } = new();

    public Models.Media.Tracks.Track Track
    {
        get => _track;
        set
        {
            if (Equals(value, _track)) return;
            _track = value;
            OnPropertyChanged();
        }
    }

    private async void GenreEventsOnGenreRemoveRequest(GenreDto genreDto)
    {
        var genreToRemove = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToRemove != null) ApplicationService.Invoke(() => Genres.Remove(genreToRemove));

        var genreToAdd = NotSelectedGenres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToAdd == null) ApplicationService.Invoke(() => NotSelectedGenres.Add(genreDto));

        try
        {
            await App.GetService().RemoveGenre(_trackId, genreDto.Id);
            TrackEvents.RemoveGenre(_trackId, genreDto);
        }
        catch
        {
            await Reload();
        }
    }

    private async void GenreEventsOnGenreAddRequest(GenreDto genreDto)
    {
        var genreToRemove = NotSelectedGenres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToRemove != null) ApplicationService.Invoke(() => NotSelectedGenres.Remove(genreToRemove));

        var genreToAdd = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToAdd == null) ApplicationService.Invoke(() => Genres.Add(genreDto));

        try
        {
            await App.GetService().AddGenre(_trackId, genreDto.Id);
            TrackEvents.AddGenre(_trackId, genreDto);
        }
        catch
        {
            await Reload();
        }
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var appService = App.GetService();
        var track = await appService.GetTrack(mediaId);
        Track = new Models.Media.Tracks.Track(track);

        ApplicationService.Invoke(() => Genres.Clear());
        foreach (var genreDto in track.Genres) ApplicationService.Invoke(() => Genres.Add(genreDto));

        IEnumerable<GenreDto> genres;
        try
        {
            genres = (await appService.GetGenres()).Where(x => track.Genres.All(g => g.Id != x.Id));
        }
        catch
        {
            return;
        }

        ApplicationService.Invoke(() => NotSelectedGenres.Clear());
        foreach (var genreDto in genres) ApplicationService.Invoke(() => NotSelectedGenres.Add(genreDto));
    }

    public override async Task Reload()
    {
        await LoadInfo(_trackId);
    }

    ~EditTrackGenreViewModel()
    {
        Dispose();
    }

    public override void Dispose()
    {
        GenreEvents.GenreAddRequest -= GenreEventsOnGenreAddRequest;
        GenreEvents.GenreRemoveRequest -= GenreEventsOnGenreRemoveRequest;

        GC.SuppressFinalize(this);
    }
}