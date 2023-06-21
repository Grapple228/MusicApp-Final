using Music.Applications.Windows.Enums;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Streaming;

namespace Music.Applications.Windows.Events;

public delegate void AlbumCreatedHandler(AlbumDto albumDto);

public delegate void AlbumDeletedHandler(Guid albumId);

public delegate void AlbumRemoveRequestHandler(Guid albumId, ContainerType containerType);

public delegate void AlbumRemovedHandler(Guid albumId, ContainerType containerType, Guid containerId);

public delegate void RequestAddToAlbumHandler(Guid albumId);

public delegate void AddedToAlbumHandler(Guid albumId, Guid mediaId, ContainerType containerType);

public static class AlbumEvents
{
    public static event AlbumDeletedHandler? AlbumDeleted;
    public static event AlbumRemoveRequestHandler? AlbumRemoveRequested;
    public static event AlbumRemovedHandler? AlbumRemoved;
    public static event RequestAddToAlbumHandler? RequestAddedToAlbum;
    public static event AddedToAlbumHandler? AddedToAlbum;
    public static event AlbumCreatedHandler? AlbumCreated;

    public static async void Remove(Guid albumId, ContainerType containerType, Guid containerId)
    {
        try
        {
            await App.GetService().RemoveAlbum(containerId, albumId, containerType);
        }
        catch
        {
            return;
        }

        AlbumRemoved?.Invoke(albumId, containerType, containerId);
    }

    public static void RequestRemove(Guid albumId, ContainerType containerType)
    {
        AlbumRemoveRequested?.Invoke(albumId, containerType);
    }

    public static void Delete(Guid albumId)
    {
        AlbumDeleted?.Invoke(albumId);
    }

    public static void RequestAddToAlbum(Guid albumId)
    {
        RequestAddedToAlbum?.Invoke(albumId);
    }

    public static async void AddToAlbum(Guid albumId, Guid mediaId, ContainerType containerType)
    {
        try
        {
            await App.GetService().AddToAlbum(albumId, mediaId, containerType);
        }
        catch
        {
            return;
        }

        AddedToAlbum?.Invoke(albumId, mediaId, containerType);
    }

    public static async void Create(AlbumDto albumDto)
    {
        AlbumCreated?.Invoke(albumDto);
    }
}