using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;

namespace Music.Users.Service.Consumers.Genres;

public class GenreCreatedConsumer : GenreCreatedConsumerBase<GenreMongoBase>
{
    public GenreCreatedConsumer(IRepository<GenreMongoBase> genresRepository) : base(genresRepository)
    {
    }
}