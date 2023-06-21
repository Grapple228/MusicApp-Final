using Music.Artists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

namespace Music.Artists.Service.Consumers.Artists;

public class ArtistNameChangedConsumer : ArtistNameChangedConsumerBase<Artist>
{
    public ArtistNameChangedConsumer(IRepository<Artist> repository) : base(repository)
    {
    }
}