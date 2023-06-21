using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackAlbumRemovedConsumerBase<TTrack> : IConsumer<TrackAlbumRemoved>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackAlbumRemovedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<TrackAlbumRemoved> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        if(!track.Albums.Contains(message.AlbumId)) return;
        track.Albums.Remove(message.AlbumId);
        await _repository.UpdateAsync(track);
    }
}