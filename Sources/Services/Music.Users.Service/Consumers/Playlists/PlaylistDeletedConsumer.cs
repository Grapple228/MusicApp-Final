using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Playlists;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Users.Service.Consumers.Playlists;

public class PlaylistDeletedConsumer : PlaylistDeletedConsumerBase<PlaylistMongoBase>
{
    private readonly IRepository<UserMongoBase> _usersRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PlaylistDeletedConsumer(IRepository<PlaylistMongoBase> repository, IRepository<UserMongoBase> usersRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _usersRepository = usersRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<PlaylistDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var user = await _usersRepository.GetAsync(x => x.Playlists.Contains(message.Id));
        if(user == null) return;
        
        user.Playlists.Remove(message.Id);
        await _usersRepository.UpdateAsync(user);
        await _publishEndpoint.PublishUserDeletedPlaylist(user, message.Id);
    }
}