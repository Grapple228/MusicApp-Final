using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackDeletedConsumerBase<TTrack> : IConsumer<TrackDeleted>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackDeletedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<TrackDeleted> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track == null) return;

        await _repository.RemoveAsync(message.Id);
    }
}