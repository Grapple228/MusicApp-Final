using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumTrackRemovedConsumerBase<TAlbum> : IConsumer<AlbumTrackRemoved>
    where TAlbum : IAlbumMongo, new()
{
    private readonly IRepository<TAlbum> _albumsRepository;

    protected AlbumTrackRemovedConsumerBase(IRepository<TAlbum> albumsRepository)
    {
        _albumsRepository = albumsRepository;
    }

    public virtual async Task Consume(ConsumeContext<AlbumTrackRemoved> context)
    {
        var message = context.Message;
        var album = await _albumsRepository.GetAsync(message.Id);
        if (album == null) return;

        if (!album.Tracks.Contains(message.TrackId)) return;

        album.Tracks.Remove(message.TrackId);
        await _albumsRepository.UpdateAsync(album);
    }
}