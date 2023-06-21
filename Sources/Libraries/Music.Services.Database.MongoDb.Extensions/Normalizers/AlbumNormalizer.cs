using Music.Services.Common;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models;
using Music.Shared.DTOs.Albums;
using Music.Shared.Services;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers;

public static class AlbumNormalizer
{
    public static AlbumDto Normalize<TTrack, TAlbum, TArtist, TGenre>(this IAlbumMongo model,
        (TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TTrack> ModelTracks,
            IReadOnlyCollection<TArtist> ModelArtists) allModels, ServiceSettings serviceSettings)
        where TTrack : ITrackMongo where TAlbum : IAlbumMongo where TArtist : IArtistMongo where TGenre : IGenreMongo
    {
        var trackDtos =
            (from track in allModels.ModelTracks.Where(x => model.Tracks.Contains(x.Id))
                let trackGenres = from genre in allModels.Genres
                    where track.Genres.Contains(genre.Id)
                    select genre.AsDto()
                let trackArtists =
                    from artist in allModels.Artists 
                    where track.OwnerId == artist.Id || track.Artists.Contains(artist.Id) select artist.AsShort()
                let trackAlbums =
                    from album in allModels.Albums where track.Albums.Contains(album.Id) select album.AsShort()
                select track.AsShortDto(allModels.Artists.First(x => x.Id == track.OwnerId).AsShort(), trackGenres, trackArtists, trackAlbums,
                    new ImagePath(ServiceNames.Tracks, serviceSettings.GatewayPath))).ToList();
        
        var artistDtos =
            (from artist in allModels.ModelArtists.Where(x => model.OwnerId == x.Id || model.Artists.Contains(x.Id))
                let artistAlbums =
                    from album in allModels.Albums where album.OwnerId == artist.Id || album.Artists.Contains(artist.Id)  select album.AsShort()
                let artistTracks =
                    from track in allModels.Tracks where track.OwnerId == artist.Id || track.Artists.Contains(artist.Id) select track.AsShort()
                select artist.AsShortDto(artistAlbums, artistTracks,
                    new ImagePath(ServiceNames.Artists, serviceSettings.GatewayPath))).ToList();
        
        return model.AsDto(allModels.Artists.First(x => x.Id == model.OwnerId).AsShort(), artistDtos, trackDtos,
            new ImagePath(ServiceNames.Albums, serviceSettings.GatewayPath));
    }
}