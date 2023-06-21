using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

public abstract class UserDeletedPlaylistConsumerBase<TUser> : IConsumer<UserDeletedPlaylist>
    where TUser : IUserMongo, new()
{
    private readonly IRepository<TUser> _repository;

    protected UserDeletedPlaylistConsumerBase(IRepository<TUser> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<UserDeletedPlaylist> context)
    {
        var message = context.Message;
        var user = await _repository.GetAsync(message.Id);
        if(user == null) return;
        if(!user.Playlists.Contains(message.PlaylistId)) return;
        user.Playlists.Remove(message.PlaylistId);
        await _repository.UpdateAsync(user);
    }
}