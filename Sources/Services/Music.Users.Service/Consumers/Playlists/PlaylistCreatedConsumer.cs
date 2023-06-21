using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Playlists;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Users.Service.Consumers.Playlists;

public class PlaylistCreatedConsumer : PlaylistCreatedConsumerBase<PlaylistMongoBase>
{
    private readonly IRepository<UserMongoBase> _usersRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PlaylistCreatedConsumer(IRepository<PlaylistMongoBase> repository, IRepository<UserMongoBase> usersRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _usersRepository = usersRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<PlaylistCreated> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var user = await _usersRepository.GetAsync(message.OwnerId);
        if(user == null) return;
        
        if(user.Playlists.Contains(message.Id)) return;
        user.Playlists.Add(message.Id);
        await _usersRepository.UpdateAsync(user);
        await _publishEndpoint.PublishUserCreatedPlaylist(user, message.Id);
    }
}