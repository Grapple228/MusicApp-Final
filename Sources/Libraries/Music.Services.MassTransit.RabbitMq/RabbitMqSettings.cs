using Music.Services.Common;

namespace Music.Services.MassTransit.RabbitMq;

public class RabbitMqSettings : ISettings
{
    public string Host { get; init; } = null!;
}