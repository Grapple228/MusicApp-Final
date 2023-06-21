using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Artists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

public abstract class ArtistNameChangedConsumerBase<TArtist> : IConsumer<ArtistNameChanged>
where TArtist : IArtistMongo, new()
{
    private readonly IRepository<TArtist> _repository;

    protected ArtistNameChangedConsumerBase(IRepository<TArtist> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<ArtistNameChanged> context)
    {
        var message = context.Message;
        var artist = await _repository.GetAsync(message.Id);
        if(artist == null) return;

        artist.Name = message.Name;
        
        await _repository.UpdateAsync(artist);
    }
}