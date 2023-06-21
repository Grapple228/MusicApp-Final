using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

public abstract class UserUsernameChangedConsumerBase<TUser> : IConsumer<UserUsernameChanged>  
    where TUser : IUserMongo, new()
{
    private readonly IRepository<TUser> _repository;

    protected UserUsernameChangedConsumerBase(IRepository<TUser> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<UserUsernameChanged> context)
    {
        var message = context.Message;
        var user = await _repository.GetAsync(message.Id);
        if(user == null) return;
        user.Username = message.Username;
        await _repository.UpdateAsync(user);
    }
}