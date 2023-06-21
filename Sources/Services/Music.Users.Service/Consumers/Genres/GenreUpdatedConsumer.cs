using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Genres;

namespace Music.Users.Service.Consumers.Genres;

public class GenreUpdatedConsumer : GenreUpdatedConsumerBase<GenreMongoBase>
{
    public GenreUpdatedConsumer(IRepository<GenreMongoBase> genresRepository) : base(genresRepository)
    {
    }
}
