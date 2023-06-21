using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

namespace Music.Tracks.Service.Consumers.Albums;

public class AlbumTitleChangedConsumer : AlbumTitleChangedConsumerBase<AlbumMongoBase>
{
    public AlbumTitleChangedConsumer(IRepository<AlbumMongoBase> repository) : base(repository)
    {
    }
}