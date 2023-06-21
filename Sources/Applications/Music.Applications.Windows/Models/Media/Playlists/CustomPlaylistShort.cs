using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Playlists;

namespace Music.Applications.Windows.Models.Short;

public class CustomPlaylistShort : IModel, IOwnerable
{
    public CustomPlaylistShort(PlaylistShort playlistShort)
    {
        Id = playlistShort.Id;
        Title = playlistShort.Title;
        OwnerId = playlistShort.OwnerId;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !IsUserOwner;
    }

    public string Title { get; }
    public Guid? OwnerId { get; }
    public Guid Id { get; init; }
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }
}