using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

public abstract class UserCreatedPlaylistConsumerBase<TUser> : IConsumer<UserCreatedPlaylist>
    where TUser : IUserMongo, new()
{
    private readonly IRepository<TUser> _repository;

    protected UserCreatedPlaylistConsumerBase(IRepository<TUser> repository)
    {
        _repository = repository;
    }
    
    public virtual async Task Consume(ConsumeContext<UserCreatedPlaylist> context)
    {
        var message = context.Message;
        var user = await _repository.GetAsync(message.Id);
        if(user == null) return;
        if(user.Playlists.Contains(message.PlaylistId)) return;
        user.Playlists.Add(message.PlaylistId);
        await _repository.UpdateAsync(user);
    }
}