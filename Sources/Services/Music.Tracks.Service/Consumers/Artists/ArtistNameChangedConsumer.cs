using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

namespace Music.Tracks.Service.Consumers.Artists;

public class ArtistNameChangedConsumer : ArtistNameChangedConsumerBase<ArtistMongoBase>
{
    public ArtistNameChangedConsumer(IRepository<ArtistMongoBase> repository) : base(repository)
    {
    }
}