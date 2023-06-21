using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

namespace Music.Users.Service.Consumers.Albums;

public class AlbumCreatedConsumer : AlbumCreatedConsumerBase<AlbumMongoBase>
{
    public AlbumCreatedConsumer(IRepository<AlbumMongoBase> repository) : base(repository)
    {
    }
}