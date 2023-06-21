using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

namespace Music.Albums.Service.Consumers.Users;

public class UserCreatedConsumer : UserCreatedConsumerBase<UserMongoBase>
{
    public UserCreatedConsumer(IRepository<UserMongoBase> repository) : base(repository)
    {
    }
}