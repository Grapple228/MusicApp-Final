using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

public abstract class UserDeletedConsumerBase<TUser> : IConsumer<UserDeleted>
    where TUser : IUserMongo, new()
{
    private readonly IRepository<TUser> _repository;

    protected UserDeletedConsumerBase(IRepository<TUser> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;
        var user = await _repository.GetAsync(message.Id);
        if(user == null) return;
        await _repository.RemoveAsync(message.Id);
    }
}