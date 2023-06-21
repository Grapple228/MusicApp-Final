using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;

namespace Music.Users.Service.Consumers.Playlists;

public class PlaylistTrackAddedConsumer : PlaylistTrackAddedConsumerBase<PlaylistMongoBase>
{
    public PlaylistTrackAddedConsumer(IRepository<PlaylistMongoBase> repository) : base(repository)
    {
    }
}