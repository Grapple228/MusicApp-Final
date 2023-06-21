using Music.Services.Models;
using Music.Services.Models.Media;

namespace Music.Services.Database.Common.Repositories;

public interface IMediaModelRepository<TMediaModel> : IRepository<TMediaModel> where TMediaModel : IMediaModelBase
{
}