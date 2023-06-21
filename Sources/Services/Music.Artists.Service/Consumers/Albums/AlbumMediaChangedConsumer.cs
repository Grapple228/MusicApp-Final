using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;
using Music.Services.Models.Media;

namespace Music.Artists.Service.Consumers.Albums;

public class AlbumMediaChangedConsumer : AlbumMediaChangedConsumerBase<AlbumInfo>
{
    public AlbumMediaChangedConsumer(IRepository<AlbumInfo> infosRepository) : base(infosRepository)
    {
    }
}