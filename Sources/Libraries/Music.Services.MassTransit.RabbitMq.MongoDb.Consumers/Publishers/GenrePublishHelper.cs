using MassTransit;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Genres;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

public static class GenrePublishHelper
{
    public static async Task PublishGenreCreated(this IPublishEndpoint publishEndpoint, IGenreMongo genre) =>
        await publishEndpoint.Publish(new GenreCreated(genre.Id, genre.Value, genre.Color));
    
    public static async Task PublishGenreDeleted(this IPublishEndpoint publishEndpoint, IGenreMongo genre) =>
        await publishEndpoint.Publish(new GenreDeleted(genre.Id));
    
    public static async Task PublishGenreUpdated(this IPublishEndpoint publishEndpoint, IGenreMongo genre) =>
        await publishEndpoint.Publish(new GenreUpdated(genre.Id, genre.Value, genre.Color));
}