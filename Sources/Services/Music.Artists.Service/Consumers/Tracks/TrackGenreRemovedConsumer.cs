using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

namespace Music.Artists.Service.Consumers.Tracks;

public class TrackGenreRemovedConsumer : TrackGenreRemovedConsumerBase<TrackMongoBase>
{
    public TrackGenreRemovedConsumer(IRepository<TrackMongoBase> repository) : base(repository)
    {
    }
}