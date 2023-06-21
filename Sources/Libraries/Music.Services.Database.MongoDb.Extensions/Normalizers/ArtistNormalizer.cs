using Music.Services.Common;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models;
using Music.Shared.DTOs.Artists;
using Music.Shared.Services;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers;

public static class ArtistNormalizer
{
    public static ArtistDto Normalize<TTrack, TAlbum, TArtist, TGenre>(this IArtistBase model,
        (TTrack[] Tracks, TAlbum[] Albums, TArtist[] Artists, TGenre[] Genres, IReadOnlyCollection<TTrack> ModelTracks,
            IReadOnlyCollection<TAlbum> ModelAlbums) allModels, ServiceSettings serviceSettings)
        where TTrack : ITrackMongo where TAlbum : IAlbumMongo where TArtist : IArtistMongo where TGenre : IGenreMongo
    {
        var trackDtos = 
            (from track in allModels.ModelTracks.Where(x => x.OwnerId == model.Id || x.Artists.Contains(model.Id))
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
        
        var albumDtos = 
            (from album in allModels.ModelAlbums.Where(x => x.OwnerId == model.Id || x.Artists.Contains(model.Id))
            let albumTracks = 
                from track in allModels.Tracks
                where album.Tracks.Contains(track.Id)
                    select track.AsShort()
            let albumArtists = 
                from artist in allModels.Artists
                where album.OwnerId == artist.Id || album.Artists.Contains(artist.Id)
                    select artist.AsShort()
            select album.AsShortDto(allModels.Artists.First(x => x.Id == album.OwnerId).AsShort(), albumArtists, albumTracks,
                new ImagePath(ServiceNames.Albums, serviceSettings.GatewayPath))).ToList();

        return model.AsDto(albumDtos, trackDtos,
            new ImagePath(ServiceNames.Artists, serviceSettings.GatewayPath));
    }
}