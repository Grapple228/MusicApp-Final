using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts.Playlists;
using Music.Services.MassTransit.Contracts.Tracks;
using Music.Services.Models.Media;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Tracks;

public abstract class TrackMediaChangedConsumerBase<TInfo> : IConsumer<TrackMediaChanged>
    where TInfo : TrackInfo, new()
{
    private readonly IRepository<TInfo> _infosRepository;

    protected TrackMediaChangedConsumerBase(IRepository<TInfo> infosRepository)
    {
        _infosRepository = infosRepository;
    }
    
    public virtual async Task Consume(ConsumeContext<TrackMediaChanged> context)
    {
        var message = context.Message;
        await message.SaveInfo(_infosRepository);
    }
}