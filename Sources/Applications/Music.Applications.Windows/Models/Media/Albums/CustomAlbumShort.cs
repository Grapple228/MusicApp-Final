using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Albums;

namespace Music.Applications.Windows.Models.Short;

public class CustomAlbumShort : IModel, IOwnerable
{
    public CustomAlbumShort(AlbumShort albumShort)
    {
        Id = albumShort.Id;
        Title = albumShort.Title;
        OwnerId = albumShort.OwnerId;
        PublicationDate = albumShort.PublicationDate;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !IsUserOwner;
    }

    public CustomAlbumShort(AlbumShortDto albumShort)
    {
        Id = albumShort.Id;
        Title = albumShort.Title;
        OwnerId = albumShort.Owner.Id;
        PublicationDate = albumShort.PublicationDate;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !IsUserOwner;
    }

    public string Title { get; init; }
    public Guid OwnerId { get; init; }
    public DateOnly PublicationDate { get; init; }
    public Guid Id { get; init; }
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }
}