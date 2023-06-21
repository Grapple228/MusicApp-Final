using Music.Applications.Windows.Enums;
using Music.Applications.Windows.Services;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Models.Popup;

public class NavigationPopupOptions
{
    public bool IsAdmin { get; init; } = ApplicationService.IsUserAdmin();
    public bool IsArtist { get; init; } = ApplicationService.IsUserArtist();
    public bool IsContainerOwner { get; init; }
    public bool IsOwner { get; init; }
    public bool IsSecondaryOwner { get; init; }
    public bool IsRemovable { get; init; }
    public RemoveType RemoveType { get; init; } = RemoveType.Nothing;
    public ContainerType ContainerType { get; init; }
    public bool IsActionsAllowed { get; init; } = true;
}