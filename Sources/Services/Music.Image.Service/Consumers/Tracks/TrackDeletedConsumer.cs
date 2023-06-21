using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Tracks;

namespace Music.Image.Service.Consumers.Tracks;

public class TrackDeletedConsumer : IConsumer<TrackDeleted>
{
    private readonly IRepository<Track> _tracksRepository;

    public TrackDeletedConsumer(IRepository<Track> tracksRepository)
    {
        _tracksRepository = tracksRepository;
    }
    
    public async Task Consume(ConsumeContext<TrackDeleted> context)
    {
        var message = context.Message;
        var track = await _tracksRepository.GetAsync(message.Id);
        if (track == null)
            return;

        await _tracksRepository.RemoveAsync(track.Id);
    }
}