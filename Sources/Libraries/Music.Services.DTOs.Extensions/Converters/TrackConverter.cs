using Music.Services.Models;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Genres;
using Music.Shared.DTOs.Tracks;

namespace Music.Services.DTOs.Extensions.Converters;

public static class TrackConverter
{
    public static TrackShortDto AsShortDto(this ITrackBase track, ArtistShort owner, IEnumerable<GenreDto> genres,
        IEnumerable<ArtistShort> artists, IEnumerable<AlbumShort> albums, ImagePath imagePath) =>
        new(track.Id, track.Title, track.Duration, track.PublicationDate, imagePath.GetPath(track.Id), owner, genres, artists, albums, track.IsPublic);

    public static TrackDto AsDto(this ITrackBase track, ArtistShort owner, IEnumerable<GenreDto> genres,
        IEnumerable<ArtistShortDto> artists, IEnumerable<AlbumShortDto> albums, ImagePath imagePath) =>
        new(track.Id, track.Title, track.Duration, track.PublicationDate, imagePath.GetPath(track.Id), owner, genres, artists, albums, track.IsPublic);
}