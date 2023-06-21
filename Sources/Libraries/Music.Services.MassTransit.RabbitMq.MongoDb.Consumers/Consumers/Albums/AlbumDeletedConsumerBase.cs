using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumDeletedConsumerBase<TAlbum> : IConsumer<AlbumDeleted>
    where TAlbum : IAlbumMongo
{
    private readonly IRepository<TAlbum> _repository;

    protected AlbumDeletedConsumerBase(IRepository<TAlbum> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<AlbumDeleted> context)
    {
        var message = context.Message;
        var album = await _repository.GetAsync(message.Id);
        if(album == null) return;
        await _repository.RemoveAsync(message.Id);
    }
}