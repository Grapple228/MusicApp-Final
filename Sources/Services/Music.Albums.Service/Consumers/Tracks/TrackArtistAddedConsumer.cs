﻿using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

namespace Music.Albums.Service.Consumers.Tracks;

public class TrackArtistAddedConsumer : TrackArtistAddedConsumerBase<TrackMongoBase>
{
    public TrackArtistAddedConsumer(IRepository<TrackMongoBase> repository) : base(repository)
    {
    }
}