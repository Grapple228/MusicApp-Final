using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;

namespace Music.Albums.Service.Consumers.Genres;

public class GenreDeletedConsumer : GenreDeletedConsumerBase<GenreMongoBase>
{
    public GenreDeletedConsumer(IRepository<GenreMongoBase> genresRepository) : base(genresRepository)
    {
    }
}