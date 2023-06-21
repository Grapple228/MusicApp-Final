using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

namespace Music.Artists.Service.Consumers.Users;

public class UserDeletedConsumer : UserDeletedConsumerBase<UserMongoBase>
{
    public UserDeletedConsumer(IRepository<UserMongoBase> repository) : base(repository)
    {
    }
}