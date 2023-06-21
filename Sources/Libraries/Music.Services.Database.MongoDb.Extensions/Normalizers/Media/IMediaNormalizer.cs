using Music.Services.Database.MongoDb.Models;
using Music.Services.Models.Media;
using Music.Shared.Common;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers.Media;

public interface IMediaNormalizer<TMediaDto, in TMediaInfo, in TModel>
    where TMediaDto : IModel
    where TModel : IModel
    where TMediaInfo : IMediaModelBase, new()
{
    Task<TMediaDto> Normalize(TModel media, Guid userId);
    Task<IReadOnlyCollection<TMediaDto>> Normalize(IReadOnlyCollection<TMediaInfo> mediaInfos, Guid userId);
    Task<IReadOnlyCollection<TMediaDto>> Normalize(IReadOnlyCollection<TModel> medias, Guid userId);
}