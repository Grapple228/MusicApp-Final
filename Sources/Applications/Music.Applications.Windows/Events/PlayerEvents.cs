using Music.Applications.Windows.Enums;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Events;

public delegate void TrackPlayRequestedHandler(Guid trackId, Guid containerId, ContainerType containerType);

public delegate void PlayContainerRequestedHandler(Guid containerId, ContainerType containerType);

public delegate void PlayChangedHandler(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying);

public delegate void PlayChangeRequestedHandler();

public delegate void ClearRequestedHandler();

public static class PlayerEvents
{
    public static event TrackPlayRequestedHandler? TrackPlayRequested;
    public static event PlayContainerRequestedHandler? ContainerPlayRequested;
    public static event PlayChangedHandler? PlayChanged;
    public static event PlayChangeRequestedHandler? PlayChangeRequested;
    public static event ClearRequestedHandler? ClearRequested;

    public static void RequestClear()
    {
        ClearRequested?.Invoke();
    }

    public static void RequestPlayChange()
    {
        PlayChangeRequested?.Invoke();
    }

    public static void RequestContainerPlay(Guid containerId, ContainerType containerType)
    {
        ContainerPlayRequested?.Invoke(containerId, containerType);
    }
    
    public static void RequestTrackPlay(Guid trackId, Guid containerId, ContainerType containerType)
    {
        TrackPlayRequested?.Invoke(trackId, containerId, containerType);
    }

    public static void ChangePlay(Guid trackId, Guid containerId, ContainerType containerType, bool isPlaying)
    {
        PlayChanged?.Invoke(trackId, containerId, containerType, isPlaying);
    }
}