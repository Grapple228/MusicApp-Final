using Music.Services.Models;

namespace Music.Services.Database.Common.Repositories;

public interface IPlaylistsRepository<TPlaylist> : IRepository<TPlaylist> where TPlaylist : IPlaylistBase
{
}