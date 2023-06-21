using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

namespace Music.Playlists.Service.Consumers.Tracks;

public class TrackPublicationDateChangedConsumer : TrackPublicationDateChangedConsumerBase<TrackMongoBase>
{
    public TrackPublicationDateChangedConsumer(IRepository<TrackMongoBase> repository) : base(repository)
    {
    }
}