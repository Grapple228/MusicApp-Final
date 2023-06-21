using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

public abstract class UserCreatedConsumerBase<TUser> : IConsumer<UserCreated>
    where TUser : IUserMongo, new()
{
    private readonly IRepository<TUser> _repository;

    protected UserCreatedConsumerBase(IRepository<TUser> repository)
    {
        _repository = repository;
    }

    public virtual async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;
        var user = await _repository.GetAsync(message.Id);
        if(user != null) return;

        user = new TUser()
        {
            Id = message.Id,
            Username = message.Username
        };
        await _repository.CreateAsync(user);
    }
}