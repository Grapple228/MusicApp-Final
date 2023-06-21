using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts;
using Music.Services.MassTransit.Contracts.Genres;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;
using Music.Tracks.Service.Models;

namespace Music.Tracks.Service.Consumers.Genres;

public class GenreDeletedConsumer : GenreDeletedConsumerBase<GenreMongoBase>
{
    private readonly IRepository<Track> _tracksRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public GenreDeletedConsumer(IRepository<GenreMongoBase> genresRepository, IRepository<Track> tracksRepository,
        IPublishEndpoint publishEndpoint) : base(genresRepository)

    {
        _tracksRepository = tracksRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<GenreDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var tracks = await _tracksRepository.GetAllAsync(x => x.Genres.Contains(message.Id));
        foreach (var track in tracks)
        {
            track.Genres.Remove(message.Id);
            await _tracksRepository.UpdateAsync(track);
            await _publishEndpoint.PublishTrackGenreRemoved(track, message.Id);
        }
    }
}