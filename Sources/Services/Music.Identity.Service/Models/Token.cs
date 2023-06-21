using Music.Shared.Common;

namespace Music.Identity.Service.Models;

public class Token : IModel
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTimeOffset ExpirationDate { get; set; }
    public DateTimeOffset RefreshDate { get; set; }
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
    public Guid Id { get; init; }
}