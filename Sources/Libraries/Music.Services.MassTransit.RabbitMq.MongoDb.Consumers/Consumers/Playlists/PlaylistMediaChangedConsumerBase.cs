using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts.Artists;
using Music.Services.MassTransit.Contracts.Playlists;
using Music.Services.Models.Media;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Playlists;

public abstract class PlaylistMediaChangedConsumerBase<TInfo> : IConsumer<PlaylistMediaChanged>
    where TInfo : PlaylistInfo, new()
{
    private readonly IRepository<TInfo> _infosRepository;

    protected PlaylistMediaChangedConsumerBase(IRepository<TInfo> infosRepository)
    {
        _infosRepository = infosRepository;
    }
    
    public virtual async Task Consume(ConsumeContext<PlaylistMediaChanged> context)
    {
        var message = context.Message;
        await message.SaveInfo(_infosRepository);
    }
}