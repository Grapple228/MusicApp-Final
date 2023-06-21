using Music.Services.Models;

namespace Music.Services.Database.Common.Repositories;

public interface IAlbumsRepository<TAlbum> : IRepository<TAlbum> where TAlbum : IAlbumBase
{
}