using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackCreatedConsumerBase<TTrack> : IConsumer<TrackCreated>
    where TTrack : ITrackMongo, new()
{
    private readonly IRepository<TTrack> _repository;

    protected TrackCreatedConsumerBase(IRepository<TTrack> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<TrackCreated> context)
    {
        var message = context.Message;
        var track = await _repository.GetAsync(message.Id);
        if(track != null) return;

        track = new TTrack()
        {
            Id = message.Id,
            OwnerId = message.OwnerId,
            Duration = message.Duration,
            Genres = message.Genres,
            Title = message.Title,
            PublicationDate = message.PublicationDate
        };

        await _repository.CreateAsync(track);
    }
}