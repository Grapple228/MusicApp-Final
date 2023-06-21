using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

namespace Music.Users.Service.Consumers.Users;

public class UserDeletedPlaylistConsumer : UserDeletedPlaylistConsumerBase<UserMongoBase>
{
    public UserDeletedPlaylistConsumer(IRepository<UserMongoBase> repository) : base(repository)
    {
    }
}