using Music.Services.Database.MongoDb.Models;
using Music.Services.Models.Media;

namespace Music.Services.Database.MongoDb.Extensions.Normalizers.Media;

public static class MediaCreator<TModel> where TModel : IMediaModelBase, new()
{
    public static TModel Create(Guid mediaId, Guid userId, bool isLiked, bool isBlocked) => 
        new()
        {
            UserId = userId,
            MediaId = mediaId,
            IsLiked = isLiked,
            IsBlocked = isBlocked
        };
    
    public static TModel Create(Guid mediaId, Guid userId) => 
        new()
        {
            UserId = userId,
            MediaId = mediaId,
            IsLiked = false,
            IsBlocked = false
        };
}