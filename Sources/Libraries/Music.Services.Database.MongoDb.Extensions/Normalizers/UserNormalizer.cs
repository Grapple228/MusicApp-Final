using Music.Services.Common;
using Music.Services.Database.MongoDb.Models;
using Music.Services.DTOs.Extensions;
using Music.Services.DTOs.Extensions.Converters;
using Music.Services.Models;
using Music.Shared.DTOs.Users;
using Music.Shared.Services;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers;

public static class UserNormalizer
{
    public static UserDto Normalize(this IUserBase model,
        (ITrackMongo[] Tracks, IUserMongo[] Users, IReadOnlyCollection<IPlaylistMongo> ModelPlaylists) allModels, 
        ServiceSettings serviceSettings)
    {
        var playlistDtos = 
            (from playlist in allModels.ModelPlaylists.Where(x => x.OwnerId == model.Id)
            let playlistUser = 
                (from user in allModels.Users
                where playlist.OwnerId == user.Id 
                    select user).First().AsShort() 
            let playlistTracks = 
                from track in allModels.Tracks
                where playlist.Tracks.Contains(track.Id)
                    select track.AsShort()
            select playlist.AsShortDto(playlistUser, playlistTracks,
                new ImagePath(ServiceNames.Playlists, serviceSettings.GatewayPath))).ToList();

        return model.AsDto(new ImagePath(ServiceNames.Users, serviceSettings.GatewayPath), playlistDtos);
    }
}