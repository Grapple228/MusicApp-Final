using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Tracks.Service.Models;

namespace Music.Tracks.Service.Consumers.Albums;

public class AlbumDeletedConsumer : AlbumDeletedConsumerBase<AlbumMongoBase>
{
    private readonly IRepository<Track> _tracksRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AlbumDeletedConsumer(IRepository<AlbumMongoBase> repository,
        IRepository<Track> tracksRepository, IPublishEndpoint publishEndpoint) : base(repository)
    {
        _tracksRepository = tracksRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<AlbumDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var tracks = await _tracksRepository.GetAllAsync(x => x.Albums.Contains(message.Id));
        foreach (var track in tracks)
        {
            track.Albums.Remove(message.Id);
            await _tracksRepository.UpdateAsync(track);
            await _publishEndpoint.PublishTrackAlbumRemoved(track, message.Id);
        }
    }
}