using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;

public abstract class PlaylistPrivacyChangedConsumerBase<TPlaylist> : IConsumer<PlaylistPrivacyChanged>
    where TPlaylist : IPlaylistMongo, new()
{
    private readonly IRepository<TPlaylist> _repository;

    protected PlaylistPrivacyChangedConsumerBase(IRepository<TPlaylist> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<PlaylistPrivacyChanged> context)
    {
        var message = context.Message;

        var playlist = await _repository.GetAsync(message.Id);
        if(playlist==null) return;

        playlist.IsPublic = message.IsPublic;
        await _repository.UpdateAsync(playlist);
    }
}