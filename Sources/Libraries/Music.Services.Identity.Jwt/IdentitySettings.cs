using Music.Services.Common;

namespace Music.Services.Identity.Jwt;

public class IdentitySettings : ISettings
{
    public string Key { get; init; } = null!;
    public double AccessValidityMinutes { get; init; } = 15;
    public double RefreshValidityDays { get; init; } = 30;
}