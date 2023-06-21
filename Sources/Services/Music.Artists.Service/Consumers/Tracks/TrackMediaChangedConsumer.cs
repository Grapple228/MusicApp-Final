using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;
using Music.Services.Models.Media;

namespace Music.Artists.Service.Consumers.Tracks;

public class TrackMediaChangedConsumer : TrackMediaChangedConsumerBase<TrackInfo>
{
    public TrackMediaChangedConsumer(IRepository<TrackInfo> infosRepository) : base(infosRepository)
    {
    }
}