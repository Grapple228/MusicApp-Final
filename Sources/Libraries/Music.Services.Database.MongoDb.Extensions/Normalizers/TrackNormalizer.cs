using Music.Services.Common;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models;
using Music.Shared.DTOs.Tracks;
using Music.Shared.Services;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers;

public static class TrackNormalizer
{
    public static TrackDto Normalize<TTrack, TAlbum, TArtist, TGenre>(this ITrackMongo model,
        (TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TArtist> ModelArtists
            , IReadOnlyCollection<TAlbum> ModelAlbums, IReadOnlyCollection<TGenre> ModelGenres) allModels,
        ServiceSettings serviceSettings) where TTrack : ITrackMongo
        where TAlbum : IAlbumMongo
        where TArtist : IArtistMongo
        where TGenre : IGenreMongo
    {
        var genreDtos = allModels.ModelGenres.Select(genre => genre.AsDto()).ToList();

        var artistDtos = 
            (from artist in allModels.ModelArtists.Where(x => model.OwnerId == x.Id || model.Artists.Contains(x.Id))
                let artistAlbums = 
                    from album in allModels.Albums
                    where album.OwnerId == artist.Id || album.Artists.Contains(artist.Id)
                    select album.AsShort()
                let artistTracks = 
                    from track in allModels.Tracks
                    where track.OwnerId == artist.Id || track.Artists.Contains(artist.Id)
                    select track.AsShort()
                select artist.AsShortDto(artistAlbums, artistTracks,
                    new ImagePath(ServiceNames.Artists, serviceSettings.GatewayPath))).ToList();

        var albumDtos = 
            (from album in allModels.ModelAlbums.Where(x => model.Albums.Contains(x.Id))
                let albumArtists = 
                    from artist in allModels.Artists
                    where album.OwnerId == artist.Id || album.Artists.Contains(artist.Id)
                    select artist.AsShort()
                let albumTracks = 
                    from track in allModels.Tracks
                    where album.Tracks.Contains(track.Id)
                    select track.AsShort()
                select album.AsShortDto(allModels.Artists.First(x => x.Id == album.OwnerId).AsShort(), albumArtists, albumTracks,
                    new ImagePath(ServiceNames.Albums, serviceSettings.GatewayPath))).ToList();

        return model.AsDto(allModels.Artists.First(x => x.Id == model.OwnerId).AsShort(), genreDtos, artistDtos, albumDtos,
            new ImagePath(ServiceNames.Tracks, serviceSettings.GatewayPath));
    }
}