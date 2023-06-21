using MassTransit;
using Music.Albums.Service.Models;
using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts.Artists;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;
using Music.Services.MassTransit.RabbitMq.Consumers.Publishers;

namespace Music.Albums.Service.Consumers.Artists;

public class ArtistDeletedConsumer : ArtistDeletedConsumerBase<ArtistMongoBase>
{
    private readonly IRepository<Album> _albumsRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ArtistDeletedConsumer(IRepository<ArtistMongoBase> repository, IRepository<Album> albumsRepository,
        IPublishEndpoint publishEndpoint) : base(repository)
    {
        _albumsRepository = albumsRepository;
        _publishEndpoint = publishEndpoint;
    }

    public override async Task Consume(ConsumeContext<ArtistDeleted> context)
    {
        await base.Consume(context);

        var message = context.Message;
        var albums = (await _albumsRepository.GetAllAsync(x => x.OwnerId == message.Id || x.Artists.Contains(message.Id))).ToList();
        
        foreach (var album in albums)
        {
            if (album.OwnerId == message.Id)
            {
                albums.Remove(album);
                await _albumsRepository.RemoveAsync(album.Id);
                await _publishEndpoint.PublishAlbumDeleted(album);
            }
            else
            {
                album.Artists.Remove(message.Id);
                await _albumsRepository.UpdateAsync(album);
                await _publishEndpoint.PublishAlbumArtistRemoved(album, message.Id);
            }
            
        }
    }
}