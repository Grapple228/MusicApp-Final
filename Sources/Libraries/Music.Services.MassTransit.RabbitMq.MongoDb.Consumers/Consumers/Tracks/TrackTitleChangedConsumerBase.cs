using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackTitleChangedConsumerBase<TTrack> : IConsumer<TrackTitleChanged>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackTitleChangedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<TrackTitleChanged> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;
        track.Title = message.Title;
        await _repository.UpdateAsync(track);
    }
}