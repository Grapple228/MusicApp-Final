using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Requests.Genres;

namespace Music.Applications.Windows.Events;

public delegate void GenreAddRequestedHandler(GenreDto genreDto);

public delegate void GenreRemoveRequestedHandler(GenreDto genreDto);

public delegate void GenreDeletedHandler(Guid genreId);

public delegate void GenreCreatedHandler(GenreDto genreDto);

public delegate void GenreUpdatedHandler(GenreDto genreDto);

public static class GenreEvents
{
    public static event GenreAddRequestedHandler? GenreAddRequest;
    public static event GenreRemoveRequestedHandler? GenreRemoveRequest;
    public static event GenreDeletedHandler? GenreDeleted;
    public static event GenreCreatedHandler? GenreCreated;
    public static event GenreUpdatedHandler? GenreUpdated;

    public static void Update(Guid genreId, GenreUpdateRequest request)
    {
        try
        {
            Task.Run(async () =>
            {
                var genre = await App.GetService().UpdateGenre(genreId, request);
                GenreUpdated?.Invoke(genre);
            });
        }
        catch
        {
            // ignored
        }
    }

    public static void Create(GenreCreateRequest request)
    {
        try
        {
            Task.Run(async () =>
            {
                var genre = await App.GetService().CreateGenre(request);
                GenreCreated?.Invoke(genre);
            });
        }
        catch
        {
            // ignored
        }
    }

    public static void Delete(Guid genreId)
    {
        try
        {
            Task.Run(() => App.GetService().DeleteGenre(genreId));
        }
        catch
        {
            return;
        }

        GenreDeleted?.Invoke(genreId);
    }

    public static void Add(GenreDto genreDto)
    {
        GenreAddRequest?.Invoke(genreDto);
    }

    public static void Remove(GenreDto genreDto)
    {
        GenreRemoveRequest?.Invoke(genreDto);
    }
}