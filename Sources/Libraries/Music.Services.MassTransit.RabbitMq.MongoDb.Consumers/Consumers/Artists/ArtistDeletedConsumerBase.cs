using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Artists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

public abstract class ArtistDeletedConsumerBase<TArtist> : IConsumer<ArtistDeleted>
where TArtist : IArtistMongo, new()
{
    private readonly IRepository<TArtist> _repository;

    protected ArtistDeletedConsumerBase(IRepository<TArtist> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<ArtistDeleted> context)
    {
        var message = context.Message;
        var artist = await _repository.GetAsync(message.Id);
        if(artist == null) return;

        await _repository.RemoveAsync(message.Id);
    }
}