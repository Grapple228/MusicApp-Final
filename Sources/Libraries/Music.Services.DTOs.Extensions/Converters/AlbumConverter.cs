using Music.Services.Models;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Tracks;

namespace Music.Services.DTOs.Extensions.Converters;

public static class AlbumConverter
{
    public static AlbumShortDto AsShortDto(this IAlbumBase album, ArtistShort owner, IEnumerable<ArtistShort> artists,
        IEnumerable<TrackShort> tracks, ImagePath imagePath) =>
        new(album.Id, album.Title, album.PublicationDate, imagePath.GetPath(album.Id), owner, artists, tracks, album.IsPublic);

    public static AlbumDto AsDto(this IAlbumBase album, ArtistShort owner, IEnumerable<ArtistShortDto> artists,
        IEnumerable<TrackShortDto> tracks, ImagePath imagePath) =>
        new(album.Id, album.Title, album.PublicationDate, imagePath.GetPath(album.Id), owner, artists, tracks, album.IsPublic);
}