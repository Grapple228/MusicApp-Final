using Music.Services.Models;

namespace Music.Services.Database.Common.Repositories;

public interface IGenresRepository<TGenre> : IRepository<TGenre> where TGenre : IGenreBase
{
    
}