using Music.Shared.Common;
using Music.Shared.Identity.Common;

namespace Music.Identity.Service.Models;

public class Role : IModel
{
    public Guid Id { get; init; }
    public string Value { get; set; } = null!;
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}