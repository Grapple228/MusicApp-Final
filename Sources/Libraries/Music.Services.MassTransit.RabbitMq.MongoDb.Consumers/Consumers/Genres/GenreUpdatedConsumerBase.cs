using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Genres;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;

public abstract class GenreUpdatedConsumerBase<TGenre> : IConsumer<GenreUpdated>
    where TGenre : IGenreMongo, new()
{
    private readonly IRepository<TGenre> _repository;

    protected GenreUpdatedConsumerBase(IRepository<TGenre> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<GenreUpdated> context)
    {
        var message = context.Message;
        var genre = await _repository.GetAsync(message.Id);
        if(genre == null) return;

        genre.Color = message.Color;
        genre.Value = message.Value;
        
        await _repository.UpdateAsync(genre);
    }
}
