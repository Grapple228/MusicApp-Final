using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;
using Music.Services.Models.Media;

namespace Music.Users.Service.Consumers.Playlists;

public class PlaylistMediaChangedConsumer : PlaylistMediaChangedConsumerBase<PlaylistInfo>
{
    public PlaylistMediaChangedConsumer(IRepository<PlaylistInfo> infosRepository) : base(infosRepository)
    {
    }
}