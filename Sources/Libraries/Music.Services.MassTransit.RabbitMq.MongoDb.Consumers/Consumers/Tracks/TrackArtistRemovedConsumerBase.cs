using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackArtistRemovedConsumerBase<TTrack> : IConsumer<TrackArtistRemoved>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackArtistRemovedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<TrackArtistRemoved> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        
        if(!track.Artists.Contains(message.ArtistId)) return;

        track.Artists.Remove(message.ArtistId);

        await _repository.UpdateAsync(track);
    }
}