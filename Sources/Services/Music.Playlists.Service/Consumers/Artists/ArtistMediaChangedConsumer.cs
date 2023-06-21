using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;
using Music.Services.Models.Media;

namespace Music.Playlists.Service.Consumers.Artists;

public class ArtistMediaChangedConsumer : ArtistMediaChangedConsumerBase<ArtistInfo>
{
    public ArtistMediaChangedConsumer(IRepository<ArtistInfo> infosRepository) : base(infosRepository)
    {
    }
}