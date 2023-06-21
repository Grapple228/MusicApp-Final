using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackAlbumAddedConsumerBase<TTrack> : IConsumer<TrackAlbumAdded>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackAlbumAddedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<TrackAlbumAdded> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        if(track.Albums.Contains(message.AlbumId)) return;
        track.Albums.Add(message.AlbumId);
        await _repository.UpdateAsync(track);
    }
}