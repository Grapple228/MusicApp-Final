using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Image.Service.Consumers.Tracks;

public class TrackCreatedConsumer : IConsumer<TrackCreated>
{
    private readonly IRepository<Track> _tracksRepository;

    public TrackCreatedConsumer(IRepository<Track> tracksRepository)
    {
        _tracksRepository = tracksRepository;
    }
    
    public async Task Consume(ConsumeContext<TrackCreated> context)
    {
        var message = context.Message;
        var track = await _tracksRepository.GetAsync(message.Id);
        if (track != null)
            return;

        track = new Track()
        {
            Id = message.Id,
            OwnerId = message.OwnerId
        };

        await _tracksRepository.CreateAsync(track);
    }
}