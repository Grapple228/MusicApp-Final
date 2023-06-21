using MassTransit;
using Music.Playlists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Playlists.Service.Consumers.Tracks;

public class TrackDeletedConsumer : TrackDeletedConsumerBase<TrackMongoBase>
{
    private readonly IRepository<Playlist> _playlistsRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public TrackDeletedConsumer(IRepository<TrackMongoBase> repository, IRepository<Playlist> playlistsRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _playlistsRepository = playlistsRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<TrackDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var playlists = await _playlistsRepository.GetAllAsync(x => x.Tracks.Contains(message.Id));
        foreach (var playlist in playlists)
        {
            playlist.Tracks.Remove(message.Id);
            await _playlistsRepository.UpdateAsync(playlist);
            await _publishEndpoint.PublishPlaylistTrackRemoved(playlist, message.Id);
        }
    }
}