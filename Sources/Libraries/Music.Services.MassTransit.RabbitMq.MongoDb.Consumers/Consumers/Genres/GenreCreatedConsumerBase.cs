﻿using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Genres;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;

public abstract class GenreCreatedConsumerBase<TGenre> : IConsumer<GenreCreated>
    where TGenre : IGenreMongo, new()
{
    private readonly IRepository<TGenre> _repository;

    protected GenreCreatedConsumerBase(IRepository<TGenre> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<GenreCreated> context)
    {
        var message = context.Message;
        var genre = await _repository.GetAsync(message.Id);
        if(genre != null) return;

        genre = new TGenre()
        {
            Id = message.Id,
            Color = message.Color,
            Value = message.Value
        };
        await _repository.CreateAsync(genre);
    }
}
