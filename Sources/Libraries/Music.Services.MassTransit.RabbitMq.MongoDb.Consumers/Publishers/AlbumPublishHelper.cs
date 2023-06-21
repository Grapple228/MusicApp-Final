using MassTransit;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

public static class AlbumPublishHelper
{
    public static async Task PublishAlbumArtistAdded(this IPublishEndpoint publishEndpoint, IAlbumMongo album, Guid artistId) =>
        await publishEndpoint.Publish(new AlbumArtistAdded(album.Id, artistId));
    
    public static async Task PublishAlbumArtistRemoved(this IPublishEndpoint publishEndpoint, IAlbumMongo album, Guid artistId) =>
        await publishEndpoint.Publish(new AlbumArtistRemoved(album.Id, artistId));

    public static async Task PublishAlbumCreated(this IPublishEndpoint publishEndpoint, IAlbumMongo album) =>
        await publishEndpoint.Publish(new AlbumCreated(album.Id, album.Title, album.OwnerId, album.PublicationDate));
    
    public static async Task PublishAlbumDeleted(this IPublishEndpoint publishEndpoint, IAlbumMongo album) =>
        await publishEndpoint.Publish(new AlbumDeleted(album.Id));
    
    public static async Task PublishAlbumMediaChanged(this IPublishEndpoint publishEndpoint, Guid albumId, Guid userId,
        bool isLiked, bool isBlocked) => await publishEndpoint.Publish(new AlbumMediaChanged(albumId, userId, isLiked, isBlocked));
    
    public static async Task PublishAlbumPrivacyChanged(this IPublishEndpoint publishEndpoint, IAlbumMongo album) =>
        await publishEndpoint.Publish(new AlbumPrivacyChanged(album.Id, album.IsPublic));
    
    public static async Task PublishAlbumPublicationDateChanged(this IPublishEndpoint publishEndpoint, IAlbumMongo album) =>
        await publishEndpoint.Publish(new AlbumPublicationDateChanged(album.Id, album.PublicationDate));
    
    public static async Task PublishAlbumTitleChanged(this IPublishEndpoint publishEndpoint, IAlbumMongo album) =>
        await publishEndpoint.Publish(new AlbumTitleChanged(album.Id, album.Title));
    
    public static async Task PublishAlbumTrackAdded(this IPublishEndpoint publishEndpoint, IAlbumMongo album, Guid trackId) =>
        await publishEndpoint.Publish(new AlbumTrackAdded(album.Id, trackId));
    
    public static async Task PublishAlbumTrackRemoved(this IPublishEndpoint publishEndpoint, IAlbumMongo album, Guid trackId) =>
        await publishEndpoint.Publish(new AlbumTrackRemoved(album.Id, trackId));
}