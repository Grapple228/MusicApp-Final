using Music.Artists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;
using Music.Services.Models.Media;

namespace Music.Artists.Service.Consumers.Artists;

public class ArtistMediaChangedConsumer : ArtistMediaChangedConsumerBase<CustomArtistInfo>
{
    public ArtistMediaChangedConsumer(IRepository<CustomArtistInfo> infosRepository) : base(infosRepository)
    {
    }
}