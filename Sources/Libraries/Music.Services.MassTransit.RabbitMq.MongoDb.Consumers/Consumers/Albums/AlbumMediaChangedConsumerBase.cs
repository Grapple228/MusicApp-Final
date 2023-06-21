using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts.Albums;
using Music.Services.Models.Media;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Albums;

public abstract class AlbumMediaChangedConsumerBase<TInfo> : IConsumer<AlbumMediaChanged>
where TInfo : AlbumInfo, new()
{
    private readonly IRepository<TInfo> _infosRepository;

    protected AlbumMediaChangedConsumerBase(IRepository<TInfo> infosRepository)
    {
        _infosRepository = infosRepository;
    }
    
    public virtual async Task Consume(ConsumeContext<AlbumMediaChanged> context)
    {
        var message = context.Message;
        await message.SaveInfo(_infosRepository);
    }
}