using MassTransit;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

public static class UserPublishHelper
{
    public static async Task PublishUserUsernameChanged(this IPublishEndpoint publishEndpoint, IUserMongo user) =>
        await publishEndpoint.Publish(new UserUsernameChanged(user.Id, user.Username));
    
    public static async Task PublishUserDeleted(this IPublishEndpoint publishEndpoint, IUserMongo user) =>
        await publishEndpoint.Publish(new UserDeleted(user.Id));
    
    public static async Task PublishUserCreated(this IPublishEndpoint publishEndpoint, IUserMongo user) =>
        await publishEndpoint.Publish(new UserCreated(user.Id, user.Username));

    public static async Task PublishUserDeletedPlaylist(this IPublishEndpoint publishEndpoint, IUserMongo user,
        Guid playlistId) => await publishEndpoint.Publish(new UserDeletedPlaylist(user.Id, playlistId));
    
    public static async Task PublishUserCreatedPlaylist(this IPublishEndpoint publishEndpoint, IUserMongo user,
        Guid playlistId) => await publishEndpoint.Publish(new UserCreatedPlaylist(user.Id, playlistId));
}