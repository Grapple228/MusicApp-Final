using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackArtistAddedConsumerBase<TTrack> : IConsumer<TrackArtistAdded>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackArtistAddedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<TrackArtistAdded> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        
        if(track.Artists.Contains(message.ArtistId)) return;

        track.Artists.Add(message.ArtistId);

        await _repository.UpdateAsync(track);
    }
}