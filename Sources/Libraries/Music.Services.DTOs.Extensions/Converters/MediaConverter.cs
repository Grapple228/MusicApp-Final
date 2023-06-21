using Music.Services.Models;
using Music.Services.Models.Media;
using Music.Shared.DTOs.Albums;
using Music.Shared.DTOs.Artists;
using Music.Shared.DTOs.Media.Models;
using Music.Shared.DTOs.Media.Models.Short;
using Music.Shared.DTOs.Playlists;
using Music.Shared.DTOs.Tracks;
using Music.Shared.Tools.Helpers;

namespace Music.Services.DTOs.Extensions.Converters;

public static class MediaConverter
{
    public static MediaTrackDto AsDto(this IMediaModelBase media, TrackDto track, IEnumerable<MediaArtistShortDto> artists, IEnumerable<MediaAlbumShortDto> albums)
    {
        return new MediaTrackDto(media.MediaId, media.IsLiked, media.IsBlocked, 
            track.Title, track.Duration, track.Duration.GetTimeString(), track.PublicationDate, track.ImagePath, track.Owner, track.Genres, artists, albums);
    }

    public static MediaAlbumDto AsDto(this IMediaModelBase media, AlbumDto album, IEnumerable<MediaArtistShortDto> artists, IEnumerable<MediaTrackShortDto> tracks)
    {
        return new MediaAlbumDto(media.MediaId, media.IsLiked,
            media.IsBlocked, album.Title, album.PublicationDate, album.ImagePath, album.Owner, artists, tracks);
    }

    public static MediaArtistDto AsDto(this IMediaModelBase media, ArtistDto artist, IEnumerable<MediaAlbumShortDto> albums, IEnumerable<MediaTrackShortDto> tracks)
    {
        return new MediaArtistDto(media.MediaId, media.IsLiked,
            media.IsBlocked, artist.Name, artist.ImagePath, albums, tracks);
    }

    public static MediaPlaylistDto AsDto(this IMediaModelBase media, PlaylistDto playlist,  IEnumerable<MediaTrackShortDto> tracks)
    {
        return new MediaPlaylistDto(media.MediaId, media.IsLiked,
            media.IsBlocked, playlist.Title, playlist.User, playlist.ImagePath, tracks);
    }

    public static MediaTrackShortDto AsShortDto(this TrackShortDto dto, IMediaModelBase media)
    {
        return new MediaTrackShortDto(media.MediaId, media.IsLiked,
            media.IsBlocked, dto.Title, dto.Duration, dto.Duration.GetTimeString(), 
            dto.PublicationDate, dto.ImagePath, dto.Owner, dto.Genres, dto.Artists, dto.Albums);
    }
    
    public static MediaArtistShortDto AsShortDto(this ArtistShortDto dto, IMediaModelBase media)
    {
        return new MediaArtistShortDto(media.MediaId, media.IsLiked,
            media.IsBlocked, dto.Name, dto.ImagePath, dto.Albums, dto.Tracks);
    }
    
    public static MediaAlbumShortDto AsShortDto(this AlbumShortDto dto, IMediaModelBase media)
    {
        return new MediaAlbumShortDto(media.MediaId, media.IsLiked,
            media.IsBlocked, dto.Title, dto.PublicationDate, dto.ImagePath, dto.Owner, dto.Artists, dto.Tracks);
    }
    
    public static MediaPlaylistShortDto AsShortDto(this PlaylistShortDto dto, IMediaModelBase media)
    {
        return new MediaPlaylistShortDto(media.MediaId, media.IsLiked,
            media.IsBlocked, dto.Title, dto.User, dto.ImagePath, dto.Tracks);
    }
}