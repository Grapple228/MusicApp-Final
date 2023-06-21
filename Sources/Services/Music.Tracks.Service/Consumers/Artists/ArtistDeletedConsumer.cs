using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Artists;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Tracks.Service.Models;

namespace Music.Tracks.Service.Consumers.Artists;

public class ArtistDeletedConsumer : ArtistDeletedConsumerBase<ArtistMongoBase>
{
    private readonly IRepository<Track> _tracksRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ArtistDeletedConsumer(IRepository<ArtistMongoBase> repository, IRepository<Track> tracksRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _tracksRepository = tracksRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<ArtistDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var tracks = (await _tracksRepository.GetAllAsync(x => x.OwnerId == message.Id || x.Artists.Contains(message.Id))).ToList();
        
        foreach (var track in tracks)
        {
            if (track.OwnerId == message.Id)
            {
                tracks.Remove(track);
                await _tracksRepository.RemoveAsync(track.Id);
                await _publishEndpoint.PublishTrackDeleted(track);
            }
            else
            {
                track.Artists.Remove(message.Id);
                await _tracksRepository.UpdateAsync(track);
                await _publishEndpoint.PublishTrackArtistRemoved(track, message.Id);
            }
            
        }
    }
}