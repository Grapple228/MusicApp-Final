using Music.Services.Database.Common.Repositories;
using Music.Services.Database.MongoDb.Models;
using Music.Services.MassTransit.Contracts;
using Music.Services.Models.Media;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers;

public static class MediaHelper
{
    internal static async Task SaveInfo<TInfo>(this IMediaChanged mediaChanged, IRepository<TInfo> infosRepository)
    where TInfo : IMediaModelBase, new()
    {
        var info = await infosRepository.GetAsync(x => x.UserId == mediaChanged.UserId && x.MediaId == mediaChanged.Id);
        
        if (!mediaChanged.IsBlocked && !mediaChanged.IsLiked)
        {
            if(info == null) return;
            await infosRepository.RemoveAsync(info.Id);
        }
        else
        {
            if (info == null)
            {
                info = new TInfo
                {
                    UserId = mediaChanged.UserId,
                    MediaId = mediaChanged.Id,
                    IsBlocked = mediaChanged.IsBlocked,
                    IsLiked = mediaChanged.IsLiked
                };
                await infosRepository.CreateAsync(info);
            }
            else
            {
                info.IsBlocked = mediaChanged.IsBlocked;
                info.IsLiked = mediaChanged.IsLiked;
                await infosRepository.UpdateAsync(info);
            }
        }
    }
}