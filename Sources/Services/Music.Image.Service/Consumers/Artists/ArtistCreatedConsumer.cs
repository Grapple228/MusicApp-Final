using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Artists;

namespace Music.Image.Service.Consumers.Artists;

public class ArtistCreatedConsumer : IConsumer<ArtistCreated>
{
    private readonly IRepository<Artist> _artistsRepository;

    public ArtistCreatedConsumer(IRepository<Artist> artistsRepository)
    {
        _artistsRepository = artistsRepository;
    }
    
    public async Task Consume(ConsumeContext<ArtistCreated> context)
    {
        var message = context.Message;
        var artist = await _artistsRepository.GetAsync(message.Id);
        if (artist != null)
            return;

        artist = new Artist()
        {
            Id = message.Id,
        };

        await _artistsRepository.CreateAsync(artist);
    }
}