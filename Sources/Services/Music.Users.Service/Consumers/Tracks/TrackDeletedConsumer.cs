using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Tracks;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Users.Service.Consumers.Tracks;

public class TrackDeletedConsumer : TrackDeletedConsumerBase<TrackMongoBase>
{
    public TrackDeletedConsumer(IRepository<TrackMongoBase> repository) : base(repository)
    {
    }
}