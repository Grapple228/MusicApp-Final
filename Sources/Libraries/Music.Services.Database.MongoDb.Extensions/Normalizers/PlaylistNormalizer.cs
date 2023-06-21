using Music.Services.Common;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models;
using Music.Shared.DTOs.Playlists;
using Music.Shared.Services;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers;

public static class PlaylistNormalizer
{
    public static PlaylistDto Normalize<TTrack, TAlbum, TArtist, TGenre, TUser>(this IPlaylistMongo model,
        (TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TTrack> ModelTracks,
            IReadOnlyCollection<TUser> ModelUsers) allModels, ServiceSettings serviceSettings)
        where TTrack : ITrackMongo
        where TAlbum : IAlbumMongo
        where TArtist : IArtistMongo
        where TGenre : IGenreMongo
        where TUser : IUserMongo

    {
        var trackDtos =
            (from track in allModels.ModelTracks.Where(x => model.Tracks.Contains(x.Id))
                let trackGenres = from genre in allModels.Genres
                    where track.Genres.Contains(genre.Id)
                    select genre.AsDto()
                let trackArtists =
                    from artist in allModels.Artists 
                    where track.OwnerId == artist.Id || track.Artists.Contains(artist.Id) 
                    select artist.AsShort()
                let trackAlbums =
                    from album in allModels.Albums 
                    where track.Albums.Contains(album.Id) 
                    select album.AsShort()
                select track.AsShortDto(allModels.Artists.First(x => x.Id == track.OwnerId).AsShort(), trackGenres, trackArtists, trackAlbums,
                    new ImagePath(ServiceNames.Tracks, serviceSettings.GatewayPath))).ToList();

        var userDto = allModels.ModelUsers.First(x => x.Id == model.OwnerId)
            .AsShortDto(new ImagePath(ServiceNames.Users, serviceSettings.GatewayPath));
        return model.AsDto(userDto, trackDtos, new ImagePath(ServiceNames.Playlists, serviceSettings.GatewayPath));
    }
}