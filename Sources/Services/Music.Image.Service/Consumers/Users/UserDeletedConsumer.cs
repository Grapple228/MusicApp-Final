using MassTransit;
using Music.Image.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Users;

namespace Music.Image.Service.Consumers.Users;

public class UserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IRepository<User> _usersRepository;

    public UserDeletedConsumer(IRepository<User> usersRepository)
    {
        _usersRepository = usersRepository;
    }
    
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;
        var user = await _usersRepository.GetAsync(message.Id);
        if (user == null)
            return;

        await _usersRepository.RemoveAsync(user.Id);
    }
}