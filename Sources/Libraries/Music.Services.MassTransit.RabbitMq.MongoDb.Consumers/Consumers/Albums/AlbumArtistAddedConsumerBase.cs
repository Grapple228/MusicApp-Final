using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumArtistAddedConsumerBase<TAlbum> : IConsumer<AlbumArtistAdded>
where TAlbum : IAlbumMongo
{
    private readonly IRepository<TAlbum> _repository;

    protected AlbumArtistAddedConsumerBase(IRepository<TAlbum> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<AlbumArtistAdded> context)
    {
        var message = context.Message;
        var album = await _repository.GetAsync(message.Id);
        if(album == null) return;
        
        if(album.Artists.Contains(message.ArtistId)) return;

        album.Artists.Add(message.ArtistId);

        await _repository.UpdateAsync(album);
    }
}