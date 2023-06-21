using Music.Shared.Common;

namespace Music.Identity.Service.Models;

public class Email : IModel
{
    public Guid Id { get; init; }
    public string Value { get; set; } = null!;
    public bool IsConfirmed { get; set; }
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}