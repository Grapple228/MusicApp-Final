using MassTransit;
using Music.Services.Database.Common.Repositories;
using Music.Services.MassTransit.Contracts.Artists;
using Music.Services.Models.Media;

namespace Music.Services.MassTransit.RabbitMq.Consumers.Consumers.Artists;

public abstract class ArtistMediaChangedConsumerBase<TInfo> : IConsumer<ArtistMediaChanged>
    where TInfo : ArtistInfo, new()
{
    private readonly IRepository<TInfo> _infosRepository;

    protected ArtistMediaChangedConsumerBase(IRepository<TInfo> infosRepository)
    {
        _infosRepository = infosRepository;
    }
    
    public virtual async Task Consume(ConsumeContext<ArtistMediaChanged> context)
    {
        var message = context.Message;
        await message.SaveInfo(_infosRepository);
    }
}