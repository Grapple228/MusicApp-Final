using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Playlists;

namespace Music.Image.Service.Consumers.Playlists;

public class PlaylistDeletedConsumer : IConsumer<PlaylistDeleted>
{
    private readonly IRepository<Playlist> _playlistsRepository;

    public PlaylistDeletedConsumer(IRepository<Playlist> playlistsRepository)
    {
        _playlistsRepository = playlistsRepository;
    }
    
    public async Task Consume(ConsumeContext<PlaylistDeleted> context)
    {
        var message = context.Message;
        var playlist = await _playlistsRepository.GetAsync(message.Id);
        if (playlist == null)
            return;

        await _playlistsRepository.RemoveAsync(playlist.Id);
    }
}