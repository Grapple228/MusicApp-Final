using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

namespace Music.Playlists.Service.Consumers.Albums;

public class AlbumTrackAddedConsumer : AlbumTrackAddedConsumerBase<AlbumMongoBase>
{
    public AlbumTrackAddedConsumer(IRepository<AlbumMongoBase> albumsRepository) : base(albumsRepository)
    {
    }
}