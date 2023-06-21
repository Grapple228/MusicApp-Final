using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackDurationChangedConsumerBase<TTrack> : IConsumer<TrackDurationChanged>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackDurationChangedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<TrackDurationChanged> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        track.Duration = message.Duration;
        await _repository.UpdateAsync(track);
    }
}