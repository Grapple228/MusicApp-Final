using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Image.Service.Consumers.Users;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IRepository<User> _usersRepository;

    public UserCreatedConsumer(IRepository<User> usersRepository)
    {
        _usersRepository = usersRepository;
    }
    
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;
        var user = await _usersRepository.GetAsync(message.Id);
        if (user != null)
            return;

        user = new User()
        {
            Id = message.Id,
        };

        await _usersRepository.CreateAsync(user);
    }
}