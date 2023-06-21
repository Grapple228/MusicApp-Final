using Music.Shared.Common;

namespace Music.Identity.Service.Models;

public class DeviceType : IModel
{
    public string Value { get; set; } = null!;
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
    public Guid Id { get; init; }
}