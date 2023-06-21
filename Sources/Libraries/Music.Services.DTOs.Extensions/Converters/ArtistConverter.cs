using Music.Services.Models;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Tracks;

namespace Music.Services.DTOs.Extensions.Converters;

public static class ArtistConverter
{
    public static ArtistShortDto AsShortDto(this IArtistBase artist, IEnumerable<AlbumShort> albums,
        IEnumerable<TrackShort> tracks, ImagePath imagePath) => new(artist.Id, artist.Name, imagePath.GetPath(artist.Id), albums, tracks);

    public static ArtistDto AsDto(this IArtistBase artist, IEnumerable<AlbumShortDto> albums,
        IEnumerable<TrackShortDto> tracks, ImagePath imagePath) => new(artist.Id, artist.Name, imagePath.GetPath(artist.Id), albums, tracks);
}