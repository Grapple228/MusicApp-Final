using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Genres;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;

public abstract class PlaylistCreatedConsumerBase<TPlaylist> : IConsumer<PlaylistCreated>
    where TPlaylist : IPlaylistMongo, new()
{
    private readonly IRepository<TPlaylist> _repository;

    protected PlaylistCreatedConsumerBase(IRepository<TPlaylist> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<PlaylistCreated> context)
    {
        var message = context.Message;
        var playlist = await _repository.GetAsync(message.Id);
        if(playlist != null) return;

        playlist = new TPlaylist()
        {
            Id = message.Id,
            Title = message.Title,
            OwnerId = message.OwnerId,
            IsPublic = message.IsPublic
        };
        await _repository.CreateAsync(playlist);
    }
}
