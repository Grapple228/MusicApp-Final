using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Albums;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Tracks.Service.Models;

namespace Music.Tracks.Service.Consumers.Albums;

public class AlbumTrackAddedConsumer : AlbumTrackAddedConsumerBase<AlbumMongoBase>
{
    private readonly IRepository<Track> _tracksRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AlbumTrackAddedConsumer(IRepository<AlbumMongoBase> albumsRepository, IRepository<Track> tracksRepository,
        IPublishEndpoint publishEndpoint) : base(albumsRepository)
    {
        _tracksRepository = tracksRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<AlbumTrackAdded> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var track = await _tracksRepository.GetAsync(message.TrackId);
        if(track == null) return;
        if (track.Albums.Contains(message.Id)) return;
        track.Albums.Add(message.Id);
        await _tracksRepository.UpdateAsync(track);
        await _publishEndpoint.PublishTrackAlbumAdded(track, message.Id);
    }
}