using Music.Applications.Windows.Core;
using Music.Applications.Windows.ViewModels.Media.Track;

namespace Music.Applications.Windows.ViewModels.Media.Album;

public class CreateAlbumViewModel : ViewModelBase
{
    private string _errorMessage;
    private DateTime? _publicationDate = DateTime.Now;
    private string _title;

    public CreateAlbumViewModel(Guid artistId, bool isArtist)
    {
        ArtistId = artistId;
        IsArtist = isArtist;
    }

    public Guid ArtistId { get; }
    public bool IsArtist { get; }

    public override string ModelName { get; protected set; } = nameof(CreateAlbumViewModel);

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

    public override bool Equals(object? obj)
    {
        return obj is CreateTrackViewModel model
               && model.ModelName == ModelName;
    }

    protected bool Equals(CreateAlbumViewModel other)
    {
        return base.Equals(other) && ArtistId.Equals(other.ArtistId) && IsArtist == other.IsArtist &&
               ModelName == other.ModelName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ArtistId, IsArtist, ModelName);
    }
}