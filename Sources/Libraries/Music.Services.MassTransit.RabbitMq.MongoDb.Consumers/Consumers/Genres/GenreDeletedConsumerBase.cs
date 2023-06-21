using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Genres;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;

public abstract class GenreDeletedConsumerBase<TGenre> : IConsumer<GenreDeleted>
    where TGenre : IGenreMongo, new()
{
    private readonly IRepository<TGenre> _repository;

    protected GenreDeletedConsumerBase(IRepository<TGenre> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<GenreDeleted> context)
    {
        var message = context.Message;
        var genre = await _repository.GetAsync(message.Id);
        if(genre == null) return;
        await _repository.RemoveAsync(message.Id);
    }
}
