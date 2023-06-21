using Music.Applications.Windows.Interfaces;
using Music.Applications.Windows.Services;
using Music.Shared.Common;
using Music.Shared.DTOs.Artists;

namespace Music.Applications.Windows.Models.Short;

public class CustomArtistShort : IModel, IOwnerable
{
    public CustomArtistShort(ArtistShort artistShort, bool isContainerOwner)
    {
        Id = artistShort.Id;
        Name = artistShort.Name;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !ApplicationService.IsSame(Id);
        IsContainerOwner = isContainerOwner;
    }

    public CustomArtistShort(ArtistShortDto artistShort, bool isContainerOwner)
    {
        Id = artistShort.Id;
        Name = artistShort.Name;
        IsUserOwner = ApplicationService.IsUserOwner(Id);
        IsSecondaryOwner = IsUserOwner;
        IsRemovable = !ApplicationService.IsSame(Id);
        IsContainerOwner = isContainerOwner;
    }

    public string Name { get; }
    public bool IsContainerOwner { get; private set; }
    public Guid Id { get; init; }
    public bool IsUserOwner { get; }
    public bool IsSecondaryOwner { get; }
    public bool IsRemovable { get; }
}