using MassTransit;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Artists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

public static class ArtistPublishHelper
{
    public static async Task PublishArtistCreated(this IPublishEndpoint publishEndpoint, IArtistMongo artist) =>
        await publishEndpoint.Publish(new ArtistCreated(artist.Id, artist.Name));
    
    public static async Task PublishArtistDeleted(this IPublishEndpoint publishEndpoint, IArtistMongo artist) =>
        await publishEndpoint.Publish(new ArtistDeleted(artist.Id));
    
    public static async Task PublishArtistMediaChanged(this IPublishEndpoint publishEndpoint, Guid artistId, Guid userId,
        bool isLiked, bool isBlocked) =>
        await publishEndpoint.Publish(new ArtistMediaChanged(artistId, userId, isLiked, isBlocked));
    
    public static async Task PublishArtistNameChanged(this IPublishEndpoint publishEndpoint, IArtistMongo artist) =>
        await publishEndpoint.Publish(new ArtistNameChanged(artist.Id, artist.Name));
}