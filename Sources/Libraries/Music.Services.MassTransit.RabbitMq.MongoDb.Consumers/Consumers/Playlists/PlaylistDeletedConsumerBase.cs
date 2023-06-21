using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;

public abstract class PlaylistDeletedConsumerBase<TPlaylist> : IConsumer<PlaylistDeleted>
    where TPlaylist : IPlaylistMongo, new()
{
    private readonly IRepository<TPlaylist> _repository;

    protected PlaylistDeletedConsumerBase(IRepository<TPlaylist> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<PlaylistDeleted> context)
    {
        var message = context.Message;
        var playlist = await _repository.GetAsync(message.Id);
        if(playlist == null) return;
        await _repository.RemoveAsync(message.Id);
    }
}
