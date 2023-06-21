using MassTransit;
using Music.Albums.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Albums.Service.Consumers.Tracks;

public class TrackDeletedConsumer : TrackDeletedConsumerBase<TrackMongoBase>
{
    private readonly IRepository<Album> _albumsRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public TrackDeletedConsumer(IRepository<TrackMongoBase> repository, IRepository<Album> albumsRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _albumsRepository = albumsRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<TrackDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var albums = await _albumsRepository.GetAllAsync(x => x.Tracks.Contains(message.Id));
        foreach (var album in albums)
        {
            album.Tracks.Remove(message.Id);
            await _albumsRepository.UpdateAsync(album);
            await _publishEndpoint.PublishAlbumTrackRemoved(album, message.Id);
        }
    }
}