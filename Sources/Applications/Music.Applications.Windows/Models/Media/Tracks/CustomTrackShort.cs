using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Tracks;

namespace Music.Applications.Windows.Models.Short;

public class CustomTrackShort : IOwnerable
{
    public CustomTrackShort(TrackShortDto trackShort)
    {
        Id = trackShort.Id;
        Title = trackShort.Title;
        OwnerId = trackShort.Owner.Id;
        PublicationDate = trackShort.PublicationDate;
        IsUserOwner = ApplicationService.IsUserOwner(OwnerId);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !IsUserOwner;
    }

    public CustomTrackShort(TrackShort trackShort)
    {
        Id = trackShort.Id;
        Title = trackShort.Title;
        OwnerId = trackShort.OwnerId;
        PublicationDate = trackShort.PublicationDate;
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