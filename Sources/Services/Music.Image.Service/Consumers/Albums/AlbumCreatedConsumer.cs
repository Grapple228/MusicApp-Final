using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Albums;

namespace Music.Image.Service.Consumers.Albums;

public class AlbumCreatedConsumer : IConsumer<AlbumCreated>
{
    private readonly IRepository<Album> _albumsRepository;

    public AlbumCreatedConsumer(IRepository<Album> albumsRepository)
    {
        _albumsRepository = albumsRepository;
    }
    
    public async Task Consume(ConsumeContext<AlbumCreated> context)
    {
        var message = context.Message;
        var album = await _albumsRepository.GetAsync(message.Id);
        if (album != null)
            return;

        album = new Album()
        {
            Id = message.Id,
            OwnerId = message.OwnerId
        };

        await _albumsRepository.CreateAsync(album);
    }
}