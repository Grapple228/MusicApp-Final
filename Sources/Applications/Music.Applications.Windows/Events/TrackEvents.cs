using Music.Applications.Windows.Enums;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Streaming;
using Music.Shared.DTOs.Tracks;

namespace Music.Applications.Windows.Events;

public delegate void TrackCreatedHandler(TrackDto track);

public delegate void TrackDeletedHandler(Guid trackId);

public delegate void TrackRemoveRequestHandler(Guid trackId, ContainerType containerType);

public delegate void TrackRemovedHandler(Guid trackId, ContainerType containerType, Guid containerId);

public delegate void RequestAddTrackHandler(Guid trackId);

public delegate void AddedTrackHandler(Guid trackId, Guid mediaId, ContainerType containerType);

public delegate void GenreAddedHandler(Guid trackId, GenreDto genreDto);

public delegate void GenreRemovedHandler(Guid trackId, GenreDto genreDto);

public delegate void TrackDurationChanged(Guid trackId, int duration);

public static class TrackEvents
{
    public static event TrackDeletedHandler? TrackDeleted;
    public static event TrackRemoveRequestHandler? TrackRemoveRequested;
    public static event TrackRemovedHandler? TrackRemoved;
    public static event TrackCreatedHandler? TrackCreated;
    public static event RequestAddTrackHandler? RequestTrackAdded;
    public static event AddedTrackHandler? TrackAdded;
    public static event GenreAddedHandler? GenreAdded;
    public static event GenreRemovedHandler? GenreRemoved;
    public static event TrackDurationChanged? DurationChanged;

    public static void ChangeDuration(Guid trackId, int duration)
    {
        DurationChanged?.Invoke(trackId, duration);
    }

    public static void AddGenre(Guid trackId, GenreDto genreDto)
    {
        GenreAdded?.Invoke(trackId, genreDto);
    }

    public static void RemoveGenre(Guid trackId, GenreDto genreDto)
    {
        GenreRemoved?.Invoke(trackId, genreDto);
    }

    public static async void Create(TrackDto track)
    {
        TrackCreated?.Invoke(track);
    }

    public static async void Remove(Guid trackId, ContainerType containerType, Guid containerId)
    {
        try
        {
            await App.GetService().RemoveTrack(containerId, trackId, containerType);
        }
        catch
        {
            return;
        }

        TrackRemoved?.Invoke(trackId, containerType, containerId);
    }

    public static void RequestRemove(Guid trackId, ContainerType containerType)
    {
        TrackRemoveRequested?.Invoke(trackId, containerType);
    }

    public static void Delete(Guid trackId)
    {
        TrackDeleted?.Invoke(trackId);
    }

    public static void RequestAddTrack(Guid trackId)
    {
        RequestTrackAdded?.Invoke(trackId);
    }


    public static async void AddTrack(Guid trackId, Guid mediaId, ContainerType containerType)
    {
        try
        {
            await App.GetService().AddTrack(trackId, mediaId, containerType);
        }
        catch
        {
            return;
        }

        TrackAdded?.Invoke(trackId, mediaId, containerType);
    }
}