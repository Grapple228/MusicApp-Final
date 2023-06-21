using MassTransit;
using Music.Playlists.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Playlists.Service.Consumers.Users;

public class UserDeletedConsumer : UserDeletedConsumerBase<UserMongoBase>
{
    private readonly IRepository<Playlist> _playlistsRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserDeletedConsumer(IRepository<UserMongoBase> repository, IRepository<Playlist> playlistsRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _playlistsRepository = playlistsRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<UserDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var playlists = await _playlistsRepository.GetAllAsync(x => x.OwnerId == message.Id);
        foreach (var playlist in playlists)
        {
            await _playlistsRepository.RemoveAsync(playlist.Id);
            await _publishEndpoint.PublishPlaylistDeleted(playlist);
        }
    }
}