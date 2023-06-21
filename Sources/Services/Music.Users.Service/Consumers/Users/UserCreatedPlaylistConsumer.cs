﻿using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Users;

namespace Music.Users.Service.Consumers.Users;

public class UserCreatedPlaylistConsumer : UserCreatedPlaylistConsumerBase<UserMongoBase>
{
    public UserCreatedPlaylistConsumer(IRepository<UserMongoBase> repository) : base(repository)
    {
    }
}