using System.Collections.ObjectModel;
using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Genres;

namespace Music.Applications.Windows.ViewModels.Media.Track;

public class CreateTrackViewModel : LoadableViewModel, IGenreSelector
{
    private string _errorMessage;
    private string _filename = "";
    private DateTime? _publicationDate = DateTime.Now;

    private string _title;

    public CreateTrackViewModel(Guid artistId, bool isArtist)
    {
        ArtistId = artistId;
        IsArtist = isArtist;

        Task.Run(() => LoadInfo(Guid.Empty));

        GenreEvents.GenreAddRequest += GenreEventsOnGenreAddRequest;
        GenreEvents.GenreRemoveRequest += GenreEventsOnGenreRemoveRequest;
    }

    public Guid ArtistId { get; }
    public bool IsArtist { get; }

    public string Filename
    {
        get => _filename;
        set
        {
            if (value == _filename) return;
            _filename = value;
            OnPropertyChanged();
        }
    }

    public DateTime? PublicationDate
    {
        get => _publicationDate;
        set
        {
            if (value.Equals(_publicationDate)) return;
            _publicationDate = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public override string ModelName { get; protected set; } = nameof(CreateTrackViewModel);

    public ObservableCollection<GenreDto> Genres { get; } = new();
    public ObservableCollection<GenreDto> NotSelectedGenres { get; } = new();

    private void GenreEventsOnGenreRemoveRequest(GenreDto genreDto)
    {
        var genreToRemove = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToRemove != null) ApplicationService.Invoke(() => Genres.Remove(genreToRemove));

        var genreToAdd = NotSelectedGenres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToAdd == null) ApplicationService.Invoke(() => NotSelectedGenres.Add(genreDto));
    }

    private void GenreEventsOnGenreAddRequest(GenreDto genreDto)
    {
        var genreToRemove = NotSelectedGenres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToRemove != null) ApplicationService.Invoke(() => NotSelectedGenres.Remove(genreToRemove));

        var genreToAdd = Genres.FirstOrDefault(x => x.Id == genreDto.Id);
        if (genreToAdd == null) ApplicationService.Invoke(() => Genres.Add(genreDto));
    }

    public override bool Equals(object? obj)
    {
        return obj is CreateTrackViewModel model
               && model.ModelName == ModelName;
    }

    protected bool Equals(CreateTrackViewModel other)
    {
        return base.Equals(other) && _title == other._title &&
               _errorMessage == other._errorMessage && Nullable.Equals(_publicationDate, other._publicationDate) &&
               _filename == other._filename && ArtistId.Equals(other.ArtistId) && IsArtist == other.IsArtist &&
               Genres.Equals(other.Genres) && NotSelectedGenres.Equals(other.NotSelectedGenres) &&
               ModelName == other.ModelName;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(ModelName);
        return hashCode.ToHashCode();
    }

    protected override async Task LoadMedia(Guid mediaId)
    {
        var genres = await App.GetService().GetGenres();

        ApplicationService.Invoke(() => NotSelectedGenres.Clear());
        ApplicationService.Invoke(() => Genres.Clear());

        foreach (var genreDto in genres) ApplicationService.Invoke(() => NotSelectedGenres.Add(genreDto));
    }

    public override async Task Reload()
    {
        await LoadInfo(Guid.Empty);
    }

    ~CreateTrackViewModel()
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