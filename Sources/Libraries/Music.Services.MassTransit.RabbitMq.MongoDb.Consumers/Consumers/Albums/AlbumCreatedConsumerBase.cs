using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumCreatedConsumerBase<TAlbum> : IConsumer<AlbumCreated>
    where TAlbum : IAlbumMongo, new()
{
    private readonly IRepository<TAlbum> _repository;

    protected AlbumCreatedConsumerBase(IRepository<TAlbum> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<AlbumCreated> context)
    {
        var message = context.Message;
        var album = await _repository.GetAsync(message.Id);
        if(album != null) return;

        album = new TAlbum()
        {
            Id = message.Id,
            Title = message.Title,
            OwnerId = message.OwnerId,
            PublicationDate = message.PublicationDate,
        };

        await _repository.CreateAsync(album);
    }
}