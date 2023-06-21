using MassTransit;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

public static class TrackPublishHelper
{
    public static async Task PublishTrackAlbumAdded(this IPublishEndpoint publishEndpoint, ITrackMongo track, Guid albumId) =>
        await publishEndpoint.Publish(new TrackAlbumAdded(track.Id, albumId));
    
    public static async Task PublishTrackAlbumRemoved(this IPublishEndpoint publishEndpoint, ITrackMongo track, Guid albumId) =>
        await publishEndpoint.Publish(new TrackAlbumRemoved(track.Id, albumId));
    
    public static async Task PublishTrackArtistAdded(this IPublishEndpoint publishEndpoint, ITrackMongo track, Guid artistId) =>
        await publishEndpoint.Publish(new TrackArtistAdded(track.Id, artistId));
    
    public static async Task PublishTrackArtistRemoved(this IPublishEndpoint publishEndpoint, ITrackMongo track, Guid artistId) =>
        await publishEndpoint.Publish(new TrackArtistRemoved(track.Id, artistId));
        
    public static async Task PublishTrackCreated(this IPublishEndpoint publishEndpoint, ITrackMongo track) =>
        await publishEndpoint.Publish(new TrackCreated(
            track.Id, track.Title, track.Duration, track.OwnerId, track.Genres, track.PublicationDate));
   
    public static async Task PublishTrackDeleted(this IPublishEndpoint publishEndpoint, ITrackMongo track) =>
        await publishEndpoint.Publish(new TrackDeleted(track.Id));

    public static async Task PublishTrackDurationChanged(this IPublishEndpoint publishEndpoint, ITrackMongo track) =>
        await publishEndpoint.Publish(new TrackDurationChanged(track.Id, track.Duration));
    
    public static async Task PublishTrackGenreAdded(this IPublishEndpoint publishEndpoint, ITrackMongo track, Guid genreId) =>
        await publishEndpoint.Publish(new TrackGenreAdded(track.Id, genreId));
    
    public static async Task PublishTrackGenreRemoved(this IPublishEndpoint publishEndpoint, ITrackMongo track, Guid genreId) =>
        await publishEndpoint.Publish(new TrackGenreRemoved(track.Id, genreId));

    public static async Task PublishTrackMediaChanged(this IPublishEndpoint publishEndpoint, Guid trackId, Guid userId,
        bool isLiked, bool isBlocked) =>
        await publishEndpoint.Publish(new TrackMediaChanged(trackId, userId, isLiked, isBlocked));

    public static async Task PublishTrackPrivacyChanged(this IPublishEndpoint publishEndpoint, ITrackMongo track) =>
        await publishEndpoint.Publish(new TrackPrivacyChanged(track.Id, track.IsPublic));
    
    public static async Task PublishTrackPublicationDateChanged(this IPublishEndpoint publishEndpoint, ITrackMongo track) =>
        await publishEndpoint.Publish(new TrackPublicationDateChanged(track.Id, track.PublicationDate));
    
    public static async Task PublishTrackTitleChanged(this IPublishEndpoint publishEndpoint, ITrackMongo track) =>
        await publishEndpoint.Publish(new TrackTitleChanged(track.Id, track.Title));
}