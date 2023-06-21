using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Image.Service.Consumers.Playlists;

public class PlaylistCreatedConsumer : IConsumer<PlaylistCreated>
{
    private readonly IRepository<Playlist> _playlistsRepository;

    public PlaylistCreatedConsumer(IRepository<Playlist> playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }
    
    public async Task Consume(ConsumeContext<PlaylistCreated> context)
    {
        var message = context.Message;
        var album = await _playlistsRepository.GetAsync(message.Id);
        if (album != null)
            return;

        album = new Playlist()
        {
            Id = message.Id,
            OwnerId = message.OwnerId
        };

        await _playlistsRepository.CreateAsync(album);
    }
}