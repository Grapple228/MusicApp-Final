﻿using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

namespace Music.Artists.Service.Consumers.Albums;

public class AlbumArtistAddedConsumer : AlbumArtistAddedConsumerBase<AlbumMongoBase>
{
    public AlbumArtistAddedConsumer(IRepository<AlbumMongoBase> repository) : base(repository)
    {
    }
}