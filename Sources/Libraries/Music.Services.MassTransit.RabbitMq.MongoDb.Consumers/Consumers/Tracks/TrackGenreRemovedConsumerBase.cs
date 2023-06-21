using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackGenreRemovedConsumerBase<TTrack> : IConsumer<TrackGenreRemoved>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackGenreRemovedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<TrackGenreRemoved> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        
        if(!track.Genres.Contains(message.GenreId)) return;

        track.Genres.Remove(message.GenreId);

        await _repository.UpdateAsync(track);
    }
}