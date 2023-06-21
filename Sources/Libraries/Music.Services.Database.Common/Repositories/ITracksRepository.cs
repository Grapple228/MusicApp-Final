using Music.Services.Models;

namespace Music.Services.Database.Common.Repositories;

public interface ITracksRepository<TTrack> : IRepository<TTrack> where TTrack : ITrackBase
{
}