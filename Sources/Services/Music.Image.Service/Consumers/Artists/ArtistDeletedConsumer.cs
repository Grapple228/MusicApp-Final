using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Artists;

namespace Music.Image.Service.Consumers.Artists;

public class ArtistDeletedConsumer : IConsumer<ArtistDeleted>
{
    private readonly IRepository<Artist> _artistsRepository;

    public ArtistDeletedConsumer(IRepository<Artist> artistsRepository)
    {
        _artistsRepository = artistsRepository;
    }
    
    public async Task Consume(ConsumeContext<ArtistDeleted> context)
    {
        var message = context.Message;
        var artist = await _artistsRepository.GetAsync(message.Id);
        if (artist == null)
            return;

        await _artistsRepository.RemoveAsync(artist.Id);
    }
}