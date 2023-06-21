using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Events;

public delegate void ArtistRemoveRequestHandler(Guid artistId, ContainerType containerType);

public delegate void ArtistRemovedHandler(Guid artistId, ContainerType containerType, Guid containerId);

public delegate void RequestAddArtistHandler(Guid artistId);

public delegate void AddedArtistHandler(Guid artistId, Guid mediaId, ContainerType containerType);

public static class ArtistEvents
{
    public static event ArtistRemoveRequestHandler? ArtistRemoveRequested;
    public static event ArtistRemovedHandler? ArtistRemoved;
    public static event RequestAddArtistHandler? RequestedAddArtist;
    public static event AddedArtistHandler? ArtistAdded;

    public static void RequestArtistAdd(Guid artistId)
    {
        RequestedAddArtist?.Invoke(artistId);
    }

    public static async void ArtistAdd(Guid artistId, Guid mediaId, ContainerType containerType)
    {
        try
        {
            await App.GetService().AddArtist(artistId, mediaId, containerType);
        }
        catch
        {
            return;
        }

        ArtistAdded?.Invoke(artistId, mediaId, containerType);
    }

    public static async void Remove(Guid artistId, ContainerType containerType, Guid containerId)
    {
        try
        {
            await App.GetService().RemoveArtist(containerId, artistId, containerType);
        }
        catch
        {
            return;
        }

        ArtistRemoved?.Invoke(artistId, containerType, containerId);
    }

    public static void RequestRemove(Guid artistId, ContainerType containerType)
    {
        ArtistRemoveRequested?.Invoke(artistId, containerType);
    }
}