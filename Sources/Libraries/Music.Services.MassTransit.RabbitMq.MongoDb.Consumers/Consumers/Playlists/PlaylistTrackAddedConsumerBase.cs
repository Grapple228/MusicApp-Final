using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;

public abstract class PlaylistTrackAddedConsumerBase<TPlaylist> : IConsumer<PlaylistTrackAdded>
    where TPlaylist : IPlaylistMongo, new()
{
    private readonly IRepository<TPlaylist> _repository;

    protected PlaylistTrackAddedConsumerBase(IRepository<TPlaylist> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<PlaylistTrackAdded> context)
    {
        var message = context.Message;
        var playlist = await _repository.GetAsync(message.Id);
        if(playlist == null) return;
        
        if(playlist.Tracks.Contains(message.TrackId)) return;

        playlist.Tracks.Add(message.TrackId);

        await _repository.UpdateAsync(playlist);
    }
}