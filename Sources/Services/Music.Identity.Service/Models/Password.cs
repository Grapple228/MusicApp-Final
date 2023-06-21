using Music.Shared.Common;

namespace Music.Identity.Service.Models;

public class Password : IModel
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public string Hash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ChangingDate { get; set; } = DateTimeOffset.UtcNow;
}