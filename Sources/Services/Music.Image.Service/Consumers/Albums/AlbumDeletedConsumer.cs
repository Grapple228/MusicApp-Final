using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Image.Service.Consumers.Albums;

public class AlbumDeletedConsumer : IConsumer<AlbumDeleted>
{
    private readonly IRepository<Album> _albumsRepository;

    public AlbumDeletedConsumer(IRepository<Album> albumsRepository)
    {
        _albumsRepository = albumsRepository;
    }
    
    public async Task Consume(ConsumeContext<AlbumDeleted> context)
    {
        var message = context.Message;
        var album = await _albumsRepository.GetAsync(message.Id);
        if (album == null)
            return;

        await _albumsRepository.RemoveAsync(album.Id);
    }
}