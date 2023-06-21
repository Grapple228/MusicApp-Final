using Music.Applications.Windows.Core;
using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Shared.DTOs.Common;

namespace Music.Applications.Windows.Models;

public abstract class ImageModel : ObservableObject, IImageModel
{
    private readonly ChangeImage _changeImage;
    private readonly Guid _imageId;
    private bool _changed;
    private string _largeImagePath = string.Empty;
    private string _mediumImagePath = string.Empty;
    private string _smallImagePath = string.Empty;

    protected ImageModel(IImageBase dto, Guid id, ChangeImage changeImage)
    {
        _changeImage = changeImage;
        _imageId = id;
        SmallImagePath = dto.SmallImagePath;
        MediumImagePath = dto.MediumImagePath;
        LargeImagePath = dto.LargeImagePath;

        switch (changeImage)
        {
            case ChangeImage.Albums:
                ImageEvents.AlbumImageChanged += ImageEventsOnAlbumImageChanged;
                break;
            case ChangeImage.Artists:
                ImageEvents.ArtistImageChanged += ImageEventsOnArtistImageChanged;
                break;
            case ChangeImage.Tracks:
                ImageEvents.TrackImageChanged += ImageEventsOnTrackImageChanged;
                break;
            case ChangeImage.Playlists:
                ImageEvents.PlaylistImageChanged += ImageEventsOnPlaylistImageChanged;
                break;
            case ChangeImage.Users:
                ImageEvents.UserImageChanged += ImageEventsOnUserImageChanged;
                break;
            default:
                return;
        }
    }

    public string ImagePath { get; set; }

    public string SmallImagePath
    {
        get => _smallImagePath;
        set
        {
            _smallImagePath = string.Empty;
            _smallImagePath = value;
            OnPropertyChanged();
        }
    }

    public string MediumImagePath
    {
        get => _mediumImagePath;
        set
        {
            _mediumImagePath = string.Empty;
            _mediumImagePath = value;
            OnPropertyChanged();
        }
    }

    public string LargeImagePath
    {
        get => _largeImagePath;
        set
        {
            _largeImagePath = string.Empty;
            _largeImagePath = value;
            OnPropertyChanged();
        }
    }

    ~ImageModel()
    {
        switch (_changeImage)
        {
            case ChangeImage.Albums:
                ImageEvents.AlbumImageChanged -= ImageEventsOnAlbumImageChanged;
                break;
            case ChangeImage.Artists:
                ImageEvents.ArtistImageChanged -= ImageEventsOnArtistImageChanged;
                break;
            case ChangeImage.Tracks:
                ImageEvents.TrackImageChanged -= ImageEventsOnTrackImageChanged;
                break;
            case ChangeImage.Playlists:
                ImageEvents.PlaylistImageChanged -= ImageEventsOnPlaylistImageChanged;
                break;
            case ChangeImage.Users:
                ImageEvents.UserImageChanged -= ImageEventsOnUserImageChanged;
                break;
        }
    }

    private void Update(Guid id)
    {
        if (id != _imageId) return;

        var small = SmallImagePath;
        var medium = MediumImagePath;
        var large = LargeImagePath;

        if (_changed)
        {
            SmallImagePath = small.Remove(SmallImagePath.Length - 1, 1);
            MediumImagePath = medium.Remove(MediumImagePath.Length - 1, 1);
            LargeImagePath = large.Remove(LargeImagePath.Length - 1, 1);
        }
        else
        {
            SmallImagePath = small + "/";
            MediumImagePath = medium + "/";
            LargeImagePath = large + "/";
        }

        _changed = !_changed;
        GC.Collect();
    }

    private void ImageEventsOnUserImageChanged(Guid id)
    {
        Update(id);
    }

    private void ImageEventsOnPlaylistImageChanged(Guid id)
    {
        Update(id);
    }

    private void ImageEventsOnTrackImageChanged(Guid id)
    {
        Update(id);
    }

    private void ImageEventsOnArtistImageChanged(Guid id)
    {
        Update(id);
    }

    private void ImageEventsOnAlbumImageChanged(Guid id)
    {
        Update(id);
    }
}