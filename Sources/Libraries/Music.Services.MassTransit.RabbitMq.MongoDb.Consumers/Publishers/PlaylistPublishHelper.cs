using MassTransit;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

public static class PlaylistPublishHelper
{
    public static async Task PublishPlaylistCreated(this IPublishEndpoint publishEndpoint, IPlaylistMongo playlist) =>
        await publishEndpoint.Publish(new PlaylistCreated(playlist.Id, playlist.Title, playlist.OwnerId, playlist.IsPublic));
    
    public static async Task PublishPlaylistDeleted(this IPublishEndpoint publishEndpoint, IPlaylistMongo playlist) =>
        await publishEndpoint.Publish(new PlaylistDeleted(playlist.Id));
    
    public static async Task PublishPlaylistMediaChanged(this IPublishEndpoint publishEndpoint, Guid playlistId, Guid userId,
        bool isLiked, bool isBlocked) => await publishEndpoint.Publish(new PlaylistMediaChanged(playlistId, userId, isLiked, isBlocked));
    
    public static async Task PublishPlaylistPrivacyChanged(this IPublishEndpoint publishEndpoint, IPlaylistMongo playlist) =>
        await publishEndpoint.Publish(new PlaylistPrivacyChanged(playlist.Id, playlist.IsPublic));
    
    public static async Task PublishPlaylistTitleChanged(this IPublishEndpoint publishEndpoint, IPlaylistMongo playlist) =>
        await publishEndpoint.Publish(new PlaylistTitleChanged(playlist.Id, playlist.Title));
    
    public static async Task PublishPlaylistTrackAdded(this IPublishEndpoint publishEndpoint, IPlaylistMongo playlist, Guid trackId) =>
        await publishEndpoint.Publish(new PlaylistTrackAdded(playlist.Id, trackId));
    
    public static async Task PublishPlaylistTrackRemoved(this IPublishEndpoint publishEndpoint, IPlaylistMongo playlist, Guid trackId) =>
        await publishEndpoint.Publish(new PlaylistTrackRemoved(playlist.Id, trackId));
}