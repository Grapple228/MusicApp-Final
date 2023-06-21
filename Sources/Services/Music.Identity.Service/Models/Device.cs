using Music.Shared.Common;

namespace Music.Identity.Service.Models;

public class Device : IModel
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public Guid TypeId { get; set; }
    public int OsVersionBuild { get; set; }
    public string OsName { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public DateTimeOffset DbAddingDate { get; init; } = DateTimeOffset.UtcNow;
}