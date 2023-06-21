using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumArtistRemovedConsumerBase<TAlbum> : IConsumer<AlbumArtistRemoved>
    where TAlbum : IAlbumMongo
{
    private readonly IRepository<TAlbum> _repository;

    protected AlbumArtistRemovedConsumerBase(IRepository<TAlbum> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<AlbumArtistRemoved> context)
    {
        var message = context.Message;
        var album = await _repository.GetAsync(message.Id);
        if(album == null) return;
        
        if(!album.Artists.Contains(message.ArtistId)) return;

        album.Artists.Remove(message.ArtistId);

        await _repository.UpdateAsync(album);
    }
}