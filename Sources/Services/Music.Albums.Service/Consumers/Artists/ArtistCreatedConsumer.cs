using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

namespace Music.Albums.Service.Consumers.Artists;

public class ArtistCreatedConsumer : ArtistCreatedConsumerBase<ArtistMongoBase>
{
    public ArtistCreatedConsumer(IRepository<ArtistMongoBase> repository) : base(repository)
    {
    }
}