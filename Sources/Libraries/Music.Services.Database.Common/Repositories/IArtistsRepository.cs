using Music.Services.Models;

namespace Music.Services.Database.Common.Repositories;

public interface IArtistsRepository<TArtist> : IRepository<TArtist> where TArtist : IArtistBase
{
    
}