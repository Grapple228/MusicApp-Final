using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

namespace Music.Users.Service.Consumers.Tracks;

public class TrackAlbumAddedConsumer : TrackAlbumAddedConsumerBase<TrackMongoBase>
{
    public TrackAlbumAddedConsumer(IRepository<TrackMongoBase> repository) : base(repository)
    {
    }
}