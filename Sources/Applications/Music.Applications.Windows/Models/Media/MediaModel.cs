using Music.Applications.Windows.Events;
using Music.Applications.Windows.Interfaces;
using Music.Shared.DTOs.Common;

namespace Music.Applications.Windows.Models.Media;

public abstract class MediaModel : ImageModel, IMediaModel
{
    private bool _isBlocked;

    private bool _isLiked;

    protected MediaModel(IImageBase dto, Guid id, ChangeImage changeImage) : base(dto, id, changeImage)
    {
        Id = id;
    }

    public Guid Id { get; init; }

    public bool IsLiked
    {
        get => _isLiked;
        set
        {
            if (value) IsBlocked = false;
            if (value == _isLiked) return;
            _isLiked = value;
            OnPropertyChanged();
        }
    }

    public bool IsBlocked
    {
        get => _isBlocked;
        set
        {
            if (value) IsLiked = false;
            if (value == _isBlocked) return;
            _isBlocked = value;
            OnPropertyChanged();
        }
    }

    public virtual void ChangeLiked()
    {
        IsLiked = !IsLiked;
    }

    public virtual void ChangeBlocked()
    {
        IsBlocked = !IsBlocked;
    }
}