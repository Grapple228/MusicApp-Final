using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

namespace Music.Artists.Service.Consumers.Albums;

public class AlbumArtistRemovedConsumer : AlbumArtistRemovedConsumerBase<AlbumMongoBase>
{
    public AlbumArtistRemovedConsumer(IRepository<AlbumMongoBase> repository) : base(repository)
    {
    }
}