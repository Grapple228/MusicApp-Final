namespace Music.Services.Common;

public class ServiceSettings : ISettings
{
    public string ServiceName { get; init; } = null!;
    public string GatewayPath { get; init; } = null!;
}