using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

namespace Music.Playlists.Service.Consumers.Albums;

public class AlbumPublicationDateChangedConsumer : AlbumPublicationDateChangedConsumerBase<AlbumMongoBase>
{
    public AlbumPublicationDateChangedConsumer(IRepository<AlbumMongoBase> repository) : base(repository)
    {
    }
}