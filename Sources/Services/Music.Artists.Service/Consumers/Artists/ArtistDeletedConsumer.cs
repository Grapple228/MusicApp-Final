using Music.Artists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

namespace Music.Artists.Service.Consumers.Artists;

public class ArtistDeletedConsumer : ArtistDeletedConsumerBase<Artist>
{
    public ArtistDeletedConsumer(IRepository<Artist> repository) : base(repository)
    {
    }
}