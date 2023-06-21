using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumTitleChangedConsumerBase<TAlbum> : IConsumer<AlbumTitleChanged>
    where TAlbum : IAlbumMongo, new()
{
    private readonly IRepository<TAlbum> _repository;

    protected AlbumTitleChangedConsumerBase(IRepository<TAlbum> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<AlbumTitleChanged> context)
    {
        var message = context.Message;

        var album = await _repository.GetAsync(message.Id);
        if(album==null) return;

        album.Title = message.Title;
        await _repository.UpdateAsync(album);
    }
}