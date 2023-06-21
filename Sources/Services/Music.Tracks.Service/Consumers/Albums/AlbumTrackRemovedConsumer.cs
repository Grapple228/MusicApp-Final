using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Tracks.Service.Models;

namespace Music.Tracks.Service.Consumers.Albums;

public class AlbumTrackRemovedConsumer : AlbumTrackRemovedConsumerBase<AlbumMongoBase>
{
    private readonly IRepository<Track> _tracksRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AlbumTrackRemovedConsumer(IRepository<AlbumMongoBase> albumsRepository, IRepository<Track> tracksRepository,
        IPublishEndpoint publishEndpoint) : base(albumsRepository)
    {
        _tracksRepository = tracksRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<AlbumTrackRemoved> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var track = await _tracksRepository.GetAsync(message.TrackId);
        if(track == null) return;
        if (!track.Albums.Contains(message.Id)) return;
        track.Albums.Remove(message.Id);
        await _tracksRepository.UpdateAsync(track);
        await _publishEndpoint.PublishTrackAlbumRemoved(track, message.Id);
    }
}