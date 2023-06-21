using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Artists;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

public abstract class ArtistCreatedConsumerBase<TArtist> : IConsumer<ArtistCreated>
where TArtist : IArtistMongo, new()
{
    private readonly IRepository<TArtist> _repository;

    protected ArtistCreatedConsumerBase(IRepository<TArtist> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<ArtistCreated> context)
    {
        var message = context.Message;
        var artist = await _repository.GetAsync(message.Id);
        if(artist != null) return;

        artist = new TArtist()
        {
            Id = message.Id,
            Name = message.Name
        };
        await _repository.CreateAsync(artist);
    }
}