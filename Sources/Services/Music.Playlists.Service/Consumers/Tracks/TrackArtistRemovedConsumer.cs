using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

namespace Music.Playlists.Service.Consumers.Tracks;

public class TrackArtistRemovedConsumer : TrackArtistRemovedConsumerBase<TrackMongoBase>
{
    public TrackArtistRemovedConsumer(IRepository<TrackMongoBase> repository) : base(repository)
    {
    }
}